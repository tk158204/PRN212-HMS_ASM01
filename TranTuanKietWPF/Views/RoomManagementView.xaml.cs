using System.Windows.Controls;

namespace TranTuanKietWPF.Views
{
    public partial class RoomManagementView : UserControl
    {
        public RoomManagementView()
        {
            InitializeComponent();
            DataContext = new ViewModels.RoomViewModel();
        }
    }
} 