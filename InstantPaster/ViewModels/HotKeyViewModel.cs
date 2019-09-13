using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace InstantPaster.ViewModels
{
    internal class HotKeyViewModel : INotifyPropertyChanged, IDataErrorInfo, IChangeTracker
    {
        public event CombinationChanged CombinationChanged;

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
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_hotKey;
        private string m_description;
        private string m_pastedText;
        private bool m_isHotKeyEditing;


        public HotKeyViewModel(string _hotKey, string _description, string _pastedText)
        {
            HotKey = _hotKey;
            Description = _description;
            PastedText = _pastedText;
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
    }
}