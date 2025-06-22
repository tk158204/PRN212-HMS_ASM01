using BusinessObjects;
using System.Windows;

namespace TranTuanKietWPF
{
    public partial class RoomDialog : Window
    {
        public ViewModels.RoomDialogViewModel ViewModel { get; private set; }

        public RoomDialog(RoomInformation room, bool isEditing)
        {
            InitializeComponent();
            ViewModel = new ViewModels.RoomDialogViewModel(room, isEditing);
            DataContext = ViewModel;
        }
    }
} 