using System.Windows.Controls;

namespace TranTuanKietWPF.Views
{
    public partial class CustomerManagementView : UserControl
    {
        public CustomerManagementView()
        {
            InitializeComponent();
            DataContext = new ViewModels.CustomerViewModel();
        }
    }
} 