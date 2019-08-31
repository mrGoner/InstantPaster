using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace InstantPaster
{
    public class HotKeyConfiguration : INotifyPropertyChanged, IDataErrorInfo
    {
        private string m_hotKey = "Ctrl+C";
        private bool m_isHotKeyEditing;

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

        public HotKeyConfiguration()
        {
            // Keyboard.PrimaryDevice.FocusedElement.KeyUp += (sender, args) => { Console.WriteLine(args.Key); };
        }

        public string Description { get; set; } = "TestDesc";
        public string PastedText { get; set; } = "Test for pasting";
        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string this[string columnName] => Validate();

        public string Error => Validate();

        private string Validate()
        {

            try
            {
                var enumerable = m_hotKey.Split('+').Select(p => Enum.Parse(typeof(Keys), p)).Cast<Keys>().ToList();

                return null;
            }
            catch (Exception e)
            {
                return "Error";
            }
        }
    }
}
