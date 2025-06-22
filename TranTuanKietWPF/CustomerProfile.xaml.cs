using System.Windows;

namespace NguyenHuanWPF
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