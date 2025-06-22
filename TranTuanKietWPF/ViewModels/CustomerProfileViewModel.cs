using BusinessObjects;
using Services;
using Repositories;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace TranTuanKietWPF.ViewModels
{
    public class CustomerProfileViewModel : ViewModelBase
    {
        private readonly CustomerService _customerService;
        private readonly BookingReservationService _bookingService;
        private readonly RoomInformationService _roomService;
        private Customer? _currentCustomer;
        private ObservableCollection<BookingReservation> _bookings;
        private ObservableCollection<RoomInformation> _availableRooms;
        private BookingReservation? _selectedBooking;
        private RoomInformation? _selectedRoom;
        private string _searchText = string.Empty;
        private string _errorMessage = string.Empty;
        private DateTime _startDate = DateTime.Today;
        private DateTime _endDate = DateTime.Today.AddDays(1);

        public Customer? CurrentCustomer
        {
            get => _currentCustomer;
            set => SetProperty(ref _currentCustomer, value);
        }

        public ObservableCollection<BookingReservation> Bookings
        {
            get => _bookings;
            set => SetProperty(ref _bookings, value);
        }

        public ObservableCollection<RoomInformation> AvailableRooms
        {
            get => _availableRooms;
            set => SetProperty(ref _availableRooms, value);
        }

        public BookingReservation? SelectedBooking
        {
            get => _selectedBooking;
            set => SetProperty(ref _selectedBooking, value);
        }

        public RoomInformation? SelectedRoom
        {
            get => _selectedRoom;
            set => SetProperty(ref _selectedRoom, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public ICommand UpdateProfileCommand { get; }
        public ICommand CancelBookingCommand { get; }
        public ICommand BookRoomCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }

        public CustomerProfileViewModel()
        {
            var customerRepository = new CustomerRepository();
            var bookingRepository = new BookingReservationRepository();
            var roomRepository = new RoomInformationRepository();
            
            _customerService = new CustomerService(customerRepository);
            _bookingService = new BookingReservationService(bookingRepository);
            _roomService = new RoomInformationService(roomRepository);
            
            _bookings = new ObservableCollection<BookingReservation>();
            _availableRooms = new ObservableCollection<RoomInformation>();

            UpdateProfileCommand = new RelayCommand(_ => ExecuteUpdateProfile());
            CancelBookingCommand = new RelayCommand(_ => ExecuteCancelBooking(), _ => SelectedBooking != null);
            BookRoomCommand = new RelayCommand(_ => ExecuteBookRoom(), _ => SelectedRoom != null);
            SearchCommand = new RelayCommand(_ => ExecuteSearch());
            RefreshCommand = new RelayCommand(_ => LoadData());

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Load current customer
                if (App.CurrentCustomerID > 0)
                {
                    CurrentCustomer = _customerService.GetCustomerByID(App.CurrentCustomerID);
                }

                // Load customer's bookings
                if (CurrentCustomer != null)
                {
                    var customerBookings = _bookingService.GetByCustomer(CurrentCustomer.CustomerID);
                    Bookings = new ObservableCollection<BookingReservation>(customerBookings);
                }

                // Load available rooms
                UpdateAvailableRooms();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
            }
        }

        private void UpdateAvailableRooms()
        {
            try
            {
                var availableRooms = _roomService.GetAvailableRooms(StartDate, EndDate);
                AvailableRooms = new ObservableCollection<RoomInformation>(availableRooms);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating available rooms: {ex.Message}";
            }
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadData();
            }
            else
            {
                try
                {
                    if (CurrentCustomer != null)
                    {
                        var searchResults = _bookingService.GetByCustomer(CurrentCustomer.CustomerID)
                            .Where(b => 
                                (b.Room?.RoomNumber?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                                (b.Room?.RoomDescription?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true))
                            .ToList();
                        
                        Bookings = new ObservableCollection<BookingReservation>(searchResults);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error searching bookings: {ex.Message}";
                }
            }
        }

        private void ExecuteUpdateProfile()
        {
            if (CurrentCustomer == null) return;

            try
            {
                _customerService.UpdateCustomer(CurrentCustomer);
                MessageBox.Show("Profile updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating profile: {ex.Message}";
            }
        }

        private void ExecuteCancelBooking()
        {
            if (SelectedBooking == null) return;

            var result = MessageBox.Show($"Are you sure you want to cancel this booking?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _bookingService.Delete(SelectedBooking.BookingReservationID);
                    LoadData();
                    MessageBox.Show("Booking cancelled successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error cancelling booking: {ex.Message}";
                }
            }
        }

        private void ExecuteBookRoom()
        {
            if (SelectedRoom == null || CurrentCustomer == null) return;

            try
            {
                // Create a new booking
                var newBooking = new BookingReservation
                {
                    CustomerID = CurrentCustomer.CustomerID,
                    RoomID = SelectedRoom.RoomID,
                    BookingDate = DateTime.Now,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    BookingDuration = (int)(EndDate - StartDate).TotalDays,
                    TotalPrice = SelectedRoom.RoomPricePerDate * (int)(EndDate - StartDate).TotalDays,
                    BookingStatus = 1, // Active
                    BookingType = 1 // Online
                };

                // Add the booking
                _bookingService.Add(newBooking);

                // Refresh data
                LoadData();

                MessageBox.Show($"Room {SelectedRoom.RoomNumber} booked successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error booking room: {ex.Message}";
            }
        }
    }
} 

