using BusinessObjects;
using System.Windows;

namespace TranTuanKietWPF
{
    public partial class BookingDialog : Window
    {
        public ViewModels.BookingDialogViewModel ViewModel { get; private set; }

        public BookingDialog(BusinessObjects.BookingReservation booking, bool isEditing)
        {
            InitializeComponent();
            ViewModel = new ViewModels.BookingDialogViewModel(booking, isEditing);
            DataContext = ViewModel;
        }
    }
} 