using System.Windows;

namespace TranTuanKietWPF
{
    public partial class CustomerProfile : Window
    {
        public CustomerProfile()
        {
            InitializeComponent();
            DataContext = new ViewModels.CustomerProfileViewModel();
        }
    }
}