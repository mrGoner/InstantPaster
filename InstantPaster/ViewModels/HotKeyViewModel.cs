using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace InstantPaster.ViewModels
{
    public class HotKeyViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public string HotKey
        {
            get => m_hotKey;
            set
            {
                if (m_hotKey != value)
                {
                    m_hotKey = value;
                    OnPropertyChanged();
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

        public string this[string _columnName] => Validate();

        public string Error => Validate();
       
        private string Validate()
        {
            try
            {
                var enumerable = m_hotKey.Split('+').Select(_p => Enum.Parse(typeof(Keys), _p)).Cast<Keys>().ToList();

                return null;
            }
            catch (Exception e)
            {
                return "Error";
            }
        }
    }
}
