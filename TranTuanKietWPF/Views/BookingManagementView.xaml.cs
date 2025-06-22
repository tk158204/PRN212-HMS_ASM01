using System.Windows.Controls;

namespace TranTuanKietWPF.Views
{
    public partial class BookingManagementView : UserControl
    {
        public BookingManagementView()
        {
            InitializeComponent();
            DataContext = new ViewModels.BookingViewModel();
        }
    }
} 