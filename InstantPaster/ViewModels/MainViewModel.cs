using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using InstantPaster.Hook;
using InstantPaster.Properties;
using InstantPaster.Settings;
using Microsoft.Expression.Interactivity.Core;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace InstantPaster.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<HotKeyViewModel> HotKeys { get; set; }

        public HotKeyViewModel SelectedHotKey
        {
            get => m_selectedKeyViewModel;
            set
            {
                m_selectedKeyViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand NewConfigurationCommand { get; set; }
        public ICommand SaveConfigurationCommand { get; }
        public ICommand SaveAsConfigurationCommand { get; }
        public ICommand LoadConfigurationCommand { get; }

        public ICommand AddHotKeyCommand { get; }
        public ICommand RemoveHotKeyCommand { get; }
        public ICommand OpenDetailsCommand { get; }

        public ICommand StopTracking { get; }
        public ICommand StartTracking { get; }

        public ActionType[] Actions { get; } = Enum.GetValues(typeof(ActionType)) as ActionType[];

        private readonly ConfigurationSerializer m_configurationSerializer;
        private readonly HookEngine m_hookEngine;
        private readonly HotKeyConfigurationFactory m_factory;
        private bool m_isDocumentLoaded;
        private HotKeyViewModel m_selectedKeyViewModel;


        public bool IsDocumentLoaded
        {
            get => m_isDocumentLoaded;

            private set
            {
                if (m_isDocumentLoaded != value)
                {
                    m_isDocumentLoaded = value;
                    OnPropertyChanged();
                }
            }
        }

        private string m_loadedDocumentPath;

        public MainViewModel()
        {
            HotKeys = new ObservableCollection<HotKeyViewModel>();
            m_configurationSerializer = new ConfigurationSerializer();
            m_hookEngine = new HookEngine();

            m_factory = new HotKeyConfigurationFactory(new Dictionary<ActionType, Action<string>>
            {
                {ActionType.InsertText, PastText},
                {ActionType.ExecutePath, Execute}
            });

            NewConfigurationCommand = new ActionCommand(ClearCurrentConfiguration);
            LoadConfigurationCommand = new ActionCommand(LoadConfiguration);
            SaveConfigurationCommand = new ActionCommand(() => SaveConfiguration(m_loadedDocumentPath));
            SaveAsConfigurationCommand = new ActionCommand(OpenSaveDialog);

            AddHotKeyCommand = new ActionCommand(() =>
            {
                var hotKeyVm = new HotKeyViewModel(string.Empty, string.Empty, string.Empty, ActionType.InsertText);
                hotKeyVm.CombinationChanged += HotKeyViewModelChanged;

                HotKeys.Add(hotKeyVm);
            });

            RemoveHotKeyCommand = new ActionCommand(() =>
            {
                SelectedHotKey.CombinationChanged -= HotKeyViewModelChanged;
                HotKeys.Remove(SelectedHotKey);
                SelectedHotKey = null;

                HotKeyViewModelChanged();
            });

            StopTracking = new ActionCommand(() => m_hookEngine.StopTracking());
            StartTracking = new ActionCommand(() => m_hookEngine.StartTracking());
            OpenDetailsCommand = new ActionCommand(OpenDetailsWindow);

            if (File.Exists(Properties.Settings.Default.LastOpenedFile))
                LoadFromFile(Properties.Settings.Default.LastOpenedFile);
        }

        private void HotKeyViewModelChanged()
        {
            var keys = HotKeys.Select(_x => m_factory.Create(_x.HotKey, _x.Content, _x.SelectedActionType)).ToList();
            m_hookEngine.SetHotKeys(keys);
            m_hookEngine.StartTracking();
        }

        private void LoadConfiguration()
        {
            try
            {
                var folderBrowser = new OpenFileDialog
                {
                    CheckFileExists = true,
                    Filter = Resources.OpenSaveDIalogFilter,
                    Multiselect = false,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                var openResult = folderBrowser.ShowDialog();

                if (openResult.HasValue && openResult.Value)
                {
                    LoadFromFile(folderBrowser.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.FailedToLoadConfiguration, Resources.ErrorTitle, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadFromFile(string _pathToFile)
        {
            var data = File.ReadAllText(_pathToFile);
            var settings = m_configurationSerializer.Deserialize(data).HotKeys;
            var hotKeyConfigurations = settings.Select(_hotKey => m_factory.Create(_hotKey)).ToList();

            m_hookEngine.SetHotKeys(hotKeyConfigurations);

            var viewModels = settings.Select(_x =>
                new HotKeyViewModel(_x.Combination, _x.Description, _x.ActionContent, _x.ActionType)).ToList();

            foreach (var hotKeyViewModel in viewModels)
            {
                hotKeyViewModel.CombinationChanged += HotKeyViewModelChanged;
            }

            HotKeys = new ObservableCollection<HotKeyViewModel>(viewModels);

            m_hookEngine.StartTracking();

            IsDocumentLoaded = true;
            m_loadedDocumentPath = _pathToFile;

            Properties.Settings.Default.LastOpenedFile = m_loadedDocumentPath;
            Properties.Settings.Default.Save();

            OnPropertyChanged(nameof(HotKeys));
        }

        private void ClearCurrentConfiguration()
        {
            m_hookEngine.StopTracking();

            foreach (var hotkey in HotKeys)
            {
                hotkey.CombinationChanged -= HotKeyViewModelChanged;
            }
            
            HotKeys.Clear();
            IsDocumentLoaded = false;

            Properties.Settings.Default.LastOpenedFile = string.Empty;
            Properties.Settings.Default.Save();
        }

        private void OpenDetailsWindow()
        {
            m_hookEngine.StopTracking();

            var detailsWindow = new DetailsWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowStyle = WindowStyle.ToolWindow
            };

            var closeAction = new Action<string>(_content =>
            {
                SelectedHotKey.Content = _content;
                detailsWindow.Close();
            });

            detailsWindow.DataContext = new DetailViewModel(closeAction, SelectedHotKey.Content);

            detailsWindow.ShowDialog();

            m_hookEngine.StartTracking();
        }

        private void SaveConfiguration(string _savePath)
        {
            try
            {
                var result = m_configurationSerializer.Serialize(new Configuration(HotKeys.Select(_hotKey =>
                    new HotKeySettings(_hotKey.HotKey, _hotKey.Description, _hotKey.SelectedActionType,
                        _hotKey.Content)).ToList()));

                File.WriteAllText(_savePath, result);

                m_loadedDocumentPath = _savePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.SaveErrorMessage, Resources.ErrorTitle, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void OpenSaveDialog()
        {
            var fileBrowser = new SaveFileDialog
            {
                CheckPathExists = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                DefaultExt = "hkconf",
                AddExtension = true,
                Filter = Resources.OpenSaveDIalogFilter
            };

            var saveResult = fileBrowser.ShowDialog();

            if (saveResult == DialogResult.OK)
            {
                SaveConfiguration(fileBrowser.FileName);
            }
        }

        private void PastText(string _content)
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(100);

                App.Current.Dispatcher.Invoke(() => 
                {
                    Clipboard.SetText(_content);
                    SendKeys.SendWait("^v");
                });
            });
        }


        private void Execute(string _content)
        {
            try
            {
                Process.Start(_content);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(Resources.FailedToExecute, _content), Resources.ErrorTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }
    }
}