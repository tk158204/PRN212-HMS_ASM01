using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TranTuanKietWPF.ViewModels
{
    public class AdminDashboardViewModel : ViewModelBase
    {
        private bool _isCustomerManagementSelected = true;
        private bool _isRoomManagementSelected = false;
        private bool _isBookingManagementSelected = false;
        private bool _isReportSelected = false;
        
        public bool IsCustomerManagementSelected
        {
            get => _isCustomerManagementSelected;
            set
            {
                if (SetProperty(ref _isCustomerManagementSelected, value) && value)
                {
                    _isRoomManagementSelected = false;
                    _isBookingManagementSelected = false;
                    _isReportSelected = false;
                    OnPropertyChanged(nameof(IsRoomManagementSelected));
                    OnPropertyChanged(nameof(IsBookingManagementSelected));
                    OnPropertyChanged(nameof(IsReportSelected));
                }
            }
        }
        
        public bool IsRoomManagementSelected
        {
            get => _isRoomManagementSelected;
            set
            {
                if (SetProperty(ref _isRoomManagementSelected, value) && value)
                {
                    _isCustomerManagementSelected = false;
                    _isBookingManagementSelected = false;
                    _isReportSelected = false;
                    OnPropertyChanged(nameof(IsCustomerManagementSelected));
                    OnPropertyChanged(nameof(IsBookingManagementSelected));
                    OnPropertyChanged(nameof(IsReportSelected));
                }
            }
        }
        
        public bool IsBookingManagementSelected
        {
            get => _isBookingManagementSelected;
            set
            {
                if (SetProperty(ref _isBookingManagementSelected, value) && value)
                {
                    _isCustomerManagementSelected = false;
                    _isRoomManagementSelected = false;
                    _isReportSelected = false;
                    OnPropertyChanged(nameof(IsCustomerManagementSelected));
                    OnPropertyChanged(nameof(IsRoomManagementSelected));
                    OnPropertyChanged(nameof(IsReportSelected));
                }
            }
        }
        
        public bool IsReportSelected
        {
            get => _isReportSelected;
            set
            {
                if (SetProperty(ref _isReportSelected, value) && value)
                {
                    _isCustomerManagementSelected = false;
                    _isRoomManagementSelected = false;
                    _isBookingManagementSelected = false;
                    OnPropertyChanged(nameof(IsCustomerManagementSelected));
                    OnPropertyChanged(nameof(IsRoomManagementSelected));
                    OnPropertyChanged(nameof(IsBookingManagementSelected));
                }
            }
        }
        
        public ICommand LogoutCommand { get; }
        
        public AdminDashboardViewModel()
        {
            // Initialize commands
            LogoutCommand = new RelayCommand(_ => Logout());
        }
        
        private void Logout()
        {
            // Open login window
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            
            // Close current window
            Application.Current.Windows[0].Close();
        }
    }
} 