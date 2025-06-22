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
    public class CustomerBookingViewModel : ViewModelBase
    {
        private readonly BookingReservationService _bookingService;
        private readonly RoomInformationService _roomService;
        private readonly RoomTypeService _roomTypeService;
        private ObservableCollection<BookingReservation> _bookings;
        private ObservableCollection<RoomInformation> _availableRooms;
        private ObservableCollection<RoomType> _roomTypes;
        private BookingReservation? _selectedBooking;
        private RoomInformation? _selectedRoom;
        private RoomType? _selectedRoomType;
        private string _searchText = string.Empty;
        private string _errorMessage = string.Empty;
        private DateTime _startDate = DateTime.Today;
        private DateTime _endDate = DateTime.Today.AddDays(1);
        private int _selectedCustomerId;

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

        public ObservableCollection<RoomType> RoomTypes
        {
            get => _roomTypes;
            set => SetProperty(ref _roomTypes, value);
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

        public RoomType? SelectedRoomType
        {
            get => _selectedRoomType;
            set => SetProperty(ref _selectedRoomType, value);
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

        public int SelectedCustomerId
        {
            get => _selectedCustomerId;
            set => SetProperty(ref _selectedCustomerId, value);
        }

        public ICommand BookRoomCommand { get; }
        public ICommand CancelBookingCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand FilterByDateRangeCommand { get; }
        public ICommand FilterByRoomTypeCommand { get; }

        public CustomerBookingViewModel(int customerId)
        {
            var bookingRepository = new BookingReservationRepository();
            var roomRepository = new RoomInformationRepository();
            var roomTypeRepository = new RoomTypeRepository();
            
            _bookingService = new BookingReservationService(bookingRepository);
            _roomService = new RoomInformationService(roomRepository);
            _roomTypeService = new RoomTypeService(roomTypeRepository);
            
            _bookings = new ObservableCollection<BookingReservation>();
            _availableRooms = new ObservableCollection<RoomInformation>();
            _roomTypes = new ObservableCollection<RoomType>();
            _selectedCustomerId = customerId;

            BookRoomCommand = new RelayCommand(_ => ExecuteBookRoom(), _ => SelectedRoom != null);
            CancelBookingCommand = new RelayCommand(_ => ExecuteCancelBooking(), _ => SelectedBooking != null);
            SearchCommand = new RelayCommand(_ => ExecuteSearch());
            RefreshCommand = new RelayCommand(_ => LoadData());
            FilterByDateRangeCommand = new RelayCommand(_ => ExecuteFilterByDateRange());
            FilterByRoomTypeCommand = new RelayCommand(_ => ExecuteFilterByRoomType());

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Load customer's bookings
                var customerBookings = _bookingService.GetByCustomer(SelectedCustomerId);
                Bookings = new ObservableCollection<BookingReservation>(customerBookings);

                // Load room types
                RoomTypes = new ObservableCollection<RoomType>(_roomTypeService.GetRoomTypes());

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
                    var searchResults = _bookingService.GetByCustomer(SelectedCustomerId)
                        .Where(b => 
                            (b.Room?.RoomNumber?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                            (b.Room?.RoomDescription?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true))
                        .ToList();
                    
                    Bookings = new ObservableCollection<BookingReservation>(searchResults);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error searching bookings: {ex.Message}";
                }
            }
        }

        private void ExecuteFilterByDateRange()
        {
            try
            {
                var filteredBookings = _bookingService.GetByCustomer(SelectedCustomerId)
                    .Where(b => b.StartDate >= StartDate && b.StartDate <= EndDate)
                    .ToList();
                
                Bookings = new ObservableCollection<BookingReservation>(filteredBookings);
                UpdateAvailableRooms();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error filtering by date range: {ex.Message}";
            }
        }

        private void ExecuteFilterByRoomType()
        {
            if (SelectedRoomType == null)
            {
                UpdateAvailableRooms();
                return;
            }

            try
            {
                var filteredRooms = _roomService.GetAvailableRooms(StartDate, EndDate)
                    .Where(r => r.RoomTypeID == SelectedRoomType.RoomTypeID)
                    .ToList();
                
                AvailableRooms = new ObservableCollection<RoomInformation>(filteredRooms);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error filtering by room type: {ex.Message}";
            }
        }

        private void ExecuteBookRoom()
        {
            if (SelectedRoom == null) return;

            try
            {
                // Create a new booking
                var newBooking = new BookingReservation
                {
                    CustomerID = SelectedCustomerId,
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

        public void SelectRoom(RoomInformation room)
        {
            SelectedRoom = room;
        }
    }
} 
