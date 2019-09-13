using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using InstantPaster.Hook;
using InstantPaster.Settings;
using Microsoft.Expression.Interactivity.Core;
using Clipboard = System.Windows.Clipboard;

namespace InstantPaster.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<HotKeyViewModel> HotKeys { get; set; }

        public HotKeyViewModel SelectedHotKey { get; set; }
        public bool HotKeyEditing { get; set; }

        public ICommand SaveConfigurationCommand { get; }
        public ICommand SaveAsConfigurationCommand { get; }
        public ICommand LoadConfigurationCommand { get; }

        public ICommand AddHotKeyCommand { get; }
        public ICommand RemoveHotKeyCommand { get; }
        public ICommand OpenDetailsCommand { get; }
        public ICommand StopTracking { get; }
        public ICommand StartTracking { get; }
        
        private readonly ConfigurationSerializer m_configurationSerializer;
        private readonly HookEngine m_hookEngine;
        private readonly HotKeyConfigurationFactory m_factory;

        public MainViewModel()
        {
            HotKeys = new ObservableCollection<HotKeyViewModel>();
            m_configurationSerializer = new ConfigurationSerializer();
            m_hookEngine = new HookEngine();
            m_factory = new HotKeyConfigurationFactory(new Dictionary<ActionType, Action<string>>
            {
                { ActionType.InsertText, PastText}
            });

            LoadConfigurationCommand = new ActionCommand(LoadConfiguration);
            SaveConfigurationCommand = new ActionCommand(SaveConfiguration);

            AddHotKeyCommand = new ActionCommand(() =>
            {
                var hotKeyVm = new HotKeyViewModel(string.Empty, string.Empty, string.Empty);
                hotKeyVm.CombinationChanged += HotKeyViewModelChanged;

                HotKeys.Add(hotKeyVm);
            });

            RemoveHotKeyCommand = new ActionCommand(() =>
            {
                SelectedHotKey.CombinationChanged -= HotKeyViewModelChanged;
                HotKeys.Remove(SelectedHotKey);
                SelectedHotKey = null;

                m_hookEngine.SetHotKeys(GenerateConfigurations());
                m_hookEngine.StartTracking();
            });

            StopTracking = new ActionCommand(() => m_hookEngine.StopTracking());
            StartTracking = new ActionCommand(() => m_hookEngine.StartTracking());
        }

        private List<HotKeyConfiguration> GenerateConfigurations()
        {
            return HotKeys.Select(_x => m_factory.Create(_x.HotKey, _x.PastedText, ActionType.InsertText)).ToList();
        }

        private void HotKeyViewModelChanged()
        {
            m_hookEngine.SetHotKeys(GenerateConfigurations());
            m_hookEngine.StartTracking();
        }

        private void LoadConfiguration()
        {
            try
            {
                var data = File.ReadAllText("test.json");
                var settings = m_configurationSerializer.Deserialize(data).HotKeys;
                var hotKeyConfigurations = settings.Select(_hotKey => m_factory.Create(_hotKey)).ToList();

                m_hookEngine.SetHotKeys(hotKeyConfigurations);

                var viewModels = settings.Select(_x =>
                    new HotKeyViewModel(_x.Combination, _x.Description, _x.ActionContent)).ToList();

                foreach (var hotKeyViewModel in viewModels)
                {
                    hotKeyViewModel.CombinationChanged += HotKeyViewModelChanged;
                }

                HotKeys = new ObservableCollection<HotKeyViewModel>(viewModels);

                m_hookEngine.StartTracking();
                OnPropertyChanged(nameof(HotKeys));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SaveConfiguration()
        {
            try
            {
                var result = m_configurationSerializer.Serialize(new Configuration(HotKeys.Select(_hotKey =>
                    new HotKeySettings(_hotKey.HotKey, _hotKey.Description, ActionType.InsertText,
                        _hotKey.PastedText)).ToList()));

                File.WriteAllText("test.json", result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void PastText(string _content)
        {
            Clipboard.SetText(_content);

            SendKeys.SendWait("^v");

            Console.WriteLine($"Sended {_content}");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }
    }
}
