using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using InstantPaster.Settings;
using Microsoft.Expression.Interactivity.Core;
using Microsoft.Win32;

namespace InstantPaster.ViewModels
{
    internal class HotKeyViewModel : INotifyPropertyChanged, IDataErrorInfo, IChangeTracker
    {
        public event CombinationChanged CombinationChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public string HotKey
        {
            get => m_hotKey;
            set
            {
                if (m_hotKey != value)
                {
                    m_hotKey = value;

                    OnPropertyChanged();
                    OnChanged();
                }
            }
        }

        public string Description
        {
            get => m_description;

            set
            {
                if (m_description != value)
                {
                    m_description = value;

                    OnPropertyChanged();
                }
            }
        }


        public string PastedText
        {
            get => m_pastedText;

            set
            {
                if (m_pastedText != value)
                {
                    m_pastedText = value;

                    OnPropertyChanged();
                    OnChanged();
                }
            }
        }

        public ActionType SelectedActionType
        {
            get => m_actionType;

            set
            {
                if (m_actionType != value)
                {
                    m_actionType = value;

                    PastedText = string.Empty;

                    OnPropertyChanged();
                }
            }
        }

        public ICommand OpenFileBrowserCommand { get; }

        private string m_hotKey;
        private string m_description;
        private string m_pastedText;
        private ActionType m_actionType;

        public HotKeyViewModel(string _hotKey, string _description, string _pastedText, ActionType _actionType)
        {
            HotKey = _hotKey;
            Description = _description;
            PastedText = _pastedText;
            m_actionType = _actionType;
            OpenFileBrowserCommand = new ActionCommand(OpenFileBrowser);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        private void OnChanged()
        {
            if (Validate())
                CombinationChanged?.Invoke();
        }

        public string this[string _columnName]
        {
            get
            {
                if (_columnName == nameof(HotKey))
                    return Validate() ? null : "Error";

                return null;
            }
        }

        public string Error => null;
       
        private bool Validate()
        {
            try
            {
                var v = m_hotKey.Split('+').Select(_p => Enum.Parse(typeof(System.Windows.Forms.Keys), _p))
                    .Cast<System.Windows.Forms.Keys>().ToList();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void OpenFileBrowser()
        {
            var folderBrowser = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "AnyFiles|*.*;",
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            var openResult = folderBrowser.ShowDialog();

            if(openResult.HasValue && openResult.Value)
            {
                PastedText = folderBrowser.FileName;
            }
        }
    }
}