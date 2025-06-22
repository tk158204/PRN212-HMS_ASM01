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
    public class BookingDialogViewModel : ViewModelBase
    {
        private readonly BookingReservationService _bookingService;
        private readonly RoomInformationService _roomService;
        private readonly CustomerService _customerService;
        private readonly RoomTypeService _roomTypeService;
        
        private BookingReservation _booking;
        private ObservableCollection<Customer> _customers;
        private ObservableCollection<RoomInformation> _availableRooms;
        private bool _isEditMode;
        private string _errorMessage = string.Empty;
        private string _dialogTitle = string.Empty;
        private decimal _pricePerNight = 0;

        public BookingReservation Booking
        {
            get => _booking;
            set => SetProperty(ref _booking, value);
        }

        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public ObservableCollection<RoomInformation> AvailableRooms
        {
            get => _availableRooms;
            set => SetProperty(ref _availableRooms, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetProperty(ref _dialogTitle, value);
        }

        public decimal PricePerNight
        {
            get => _pricePerNight;
            set => SetProperty(ref _pricePerNight, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public BookingDialogViewModel(BookingReservation booking, bool isEditMode)
        {
            var bookingRepository = new BookingReservationRepository();
            var roomRepository = new RoomInformationRepository();
            var customerRepository = new CustomerRepository();
            var roomTypeRepository = new RoomTypeRepository();
            
            _bookingService = new BookingReservationService(bookingRepository);
            _roomService = new RoomInformationService(roomRepository);
            _customerService = new CustomerService(customerRepository);
            _roomTypeService = new RoomTypeService(roomTypeRepository);
            
            _booking = booking;
            _isEditMode = isEditMode;
            _dialogTitle = isEditMode ? "Edit Booking" : "Add New Booking";
            _customers = new ObservableCollection<Customer>();
            _availableRooms = new ObservableCollection<RoomInformation>();

            SaveCommand = new RelayCommand(_ => ExecuteSave());
            CancelCommand = new RelayCommand(_ => ExecuteCancel());

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Load customers
                var customers = _customerService.GetCustomers().Where(c => c.CustomerStatus == 1).ToList();
                Customers = new ObservableCollection<Customer>(customers);

                // Load available rooms
                UpdateAvailableRooms();

                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"LoadData error: {ex}");
            }
        }

        private void UpdateAvailableRooms()
        {
            try
            {
                // Get all active rooms
                var allRooms = _roomService.GetRooms().Where(r => r.RoomStatus == 1).ToList();
                
                // Get all bookings that overlap with the selected date range
                var overlappingBookings = _bookingService.GetBookings().Where(b => 
                    b.BookingStatus == 1 && // Only consider active bookings
                    b.BookingReservationID != Booking.BookingReservationID && // Exclude current booking if editing
                    ((b.StartDate <= Booking.StartDate && b.EndDate > Booking.StartDate) || // Booking starts before and ends during/after our period
                     (b.StartDate >= Booking.StartDate && b.StartDate < Booking.EndDate) || // Booking starts during our period
                     (b.StartDate <= Booking.StartDate && b.EndDate >= Booking.EndDate) || // Booking completely encompasses our period
                     (b.StartDate >= Booking.StartDate && b.EndDate <= Booking.EndDate))    // Booking is completely within our period
                ).ToList();
                
                // Get IDs of rooms that are booked during the selected period
                var bookedRoomIds = overlappingBookings.Select(b => b.RoomID).Distinct().ToList();
                
                // Filter out booked rooms
                var availableRooms = allRooms.Where(r => !bookedRoomIds.Contains(r.RoomID)).ToList();
                
                // Load room types for available rooms
                foreach (var room in availableRooms)
                {
                    room.RoomType = _roomTypeService.GetRoomTypeByID(room.RoomTypeID);
                }
                
                AvailableRooms = new ObservableCollection<RoomInformation>(availableRooms);
                
                // Update price if room is selected
                UpdatePrice();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating available rooms: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"UpdateAvailableRooms error: {ex}");
            }
        }

        private void UpdatePrice()
        {
            if (Booking.RoomID > 0)
            {
                var room = _roomService.GetRoomByID(Booking.RoomID);
                if (room != null)
                {
                    PricePerNight = room.RoomPricePerDate;
                    
                    // Calculate booking duration
                    TimeSpan duration = Booking.EndDate - Booking.StartDate;
                    int days = Math.Max(1, (int)duration.TotalDays);
                    
                    Booking.BookingDuration = days;
                    Booking.TotalPrice = PricePerNight * days;
                }
            }
            else
            {
                PricePerNight = 0;
                Booking.TotalPrice = 0;
            }
        }

        private void ExecuteSave()
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate required fields
                if (Booking.CustomerID <= 0)
                {
                    ErrorMessage = "Please select a customer.";
                    return;
                }

                if (Booking.RoomID <= 0)
                {
                    ErrorMessage = "Please select a room.";
                    return;
                }

                if (Booking.StartDate >= Booking.EndDate)
                {
                    ErrorMessage = "Check-out date must be after check-in date.";
                    return;
                }

                if (Booking.StartDate < DateTime.Today)
                {
                    ErrorMessage = "Check-in date cannot be in the past.";
                    return;
                }

                // Calculate final price
                UpdatePrice();

                // Set booking date to today if it's a new booking
                if (!IsEditMode)
                {
                    Booking.BookingDate = DateTime.Now;
                }

                // Close dialog with success
                CloseDialog(true);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving booking: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"ExecuteSave error: {ex}");
            }
        }

        private void ExecuteCancel()
        {
            CloseDialog(false);
        }

        private void CloseDialog(bool result)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = result;
                    window.Close();
                    break;
                }
            }
        }
    }
} 