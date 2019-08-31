using System.Windows;
using InstantPaster.Hook;
using InstantPaster.ViewModels;

namespace InstantPaster
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
