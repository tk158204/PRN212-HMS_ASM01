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
        private UserControl _currentViewContent;
        private string _currentView = "Customers";
        
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

        public UserControl CurrentViewContent
        {
            get => _currentViewContent;
            set => SetProperty(ref _currentViewContent, value);
        }

        public string CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }
        
        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }
        
        public AdminDashboardViewModel()
        {
            // Initialize commands
            NavigateCommand = new RelayCommand(parameter => Navigate(parameter));
            LogoutCommand = new RelayCommand(_ => Logout());
            
            // Set default view
            Navigate("Customers");
        }

        private void Navigate(object? parameter)
        {
            if (parameter is string viewName)
            {
                CurrentView = viewName;
                
                switch (viewName)
                {
                    case "Customers":
                        IsCustomerManagementSelected = true;
                        CurrentViewContent = new Views.CustomerManagementView();
                        break;
                    case "Rooms":
                        IsRoomManagementSelected = true;
                        CurrentViewContent = new Views.RoomManagementView();
                        break;
                    case "Bookings":
                        IsBookingManagementSelected = true;
                        CurrentViewContent = new Views.BookingManagementView();
                        break;
                    case "Reports":
                        IsReportSelected = true;
                        CurrentViewContent = new Views.ReportView();
                        break;
                }
            }
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