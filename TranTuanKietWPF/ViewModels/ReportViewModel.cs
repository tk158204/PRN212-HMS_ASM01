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
    public class ReportViewModel : ViewModelBase
    {
        private readonly BookingReservationService _bookingService;
        private readonly RoomInformationService _roomService;
        private readonly RoomTypeService _roomTypeService;
        private readonly CustomerService _customerService;
        private ObservableCollection<BookingReservation> _bookings;
        private ObservableCollection<RoomInformation> _rooms;
        private ObservableCollection<RoomType> _roomTypes;
        private string _selectedReportType = "Revenue";
        private DateTime _startDate = DateTime.Today.AddDays(-30);
        private DateTime _endDate = DateTime.Today;
        private string _errorMessage = string.Empty;
        private string _reportContent = string.Empty;

        public ObservableCollection<BookingReservation> Bookings
        {
            get => _bookings;
            set => SetProperty(ref _bookings, value);
        }

        public ObservableCollection<RoomInformation> Rooms
        {
            get => _rooms;
            set => SetProperty(ref _rooms, value);
        }

        public ObservableCollection<RoomType> RoomTypes
        {
            get => _roomTypes;
            set => SetProperty(ref _roomTypes, value);
        }

        public string SelectedReportType
        {
            get => _selectedReportType;
            set => SetProperty(ref _selectedReportType, value);
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

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string ReportContent
        {
            get => _reportContent;
            set => SetProperty(ref _reportContent, value);
        }

        public decimal TotalRevenue
        {
            get => _bookings?.Sum(b => b.TotalPrice) ?? 0;
        }

        public int TotalBookings
        {
            get => _bookings?.Count ?? 0;
        }

        public int ActiveBookings
        {
            get => _bookings?.Count(b => b.BookingStatus == 1) ?? 0;
        }

        public int CompletedBookings
        {
            get => _bookings?.Count(b => b.BookingStatus == 2) ?? 0;
        }

        public int CancelledBookings
        {
            get => _bookings?.Count(b => b.BookingStatus == 3) ?? 0;
        }

        public int OnlineBookings
        {
            get => _bookings?.Count(b => b.BookingType == 1) ?? 0;
        }

        public int OfflineBookings
        {
            get => _bookings?.Count(b => b.BookingType == 2) ?? 0;
        }

        public ICommand GenerateReportCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand RevenueReportCommand { get; }
        public ICommand BookingReportCommand { get; }
        public ICommand RoomReportCommand { get; }
        public ICommand CustomerReportCommand { get; }

        public ReportViewModel()
        {
            var bookingRepository = new BookingReservationRepository();
            var roomRepository = new RoomInformationRepository();
            var roomTypeRepository = new RoomTypeRepository();
            var customerRepository = new CustomerRepository();
            
            _bookingService = new BookingReservationService(bookingRepository);
            _roomService = new RoomInformationService(roomRepository);
            _roomTypeService = new RoomTypeService(roomTypeRepository);
            _customerService = new CustomerService(customerRepository);
            
            _bookings = new ObservableCollection<BookingReservation>();
            _rooms = new ObservableCollection<RoomInformation>();
            _roomTypes = new ObservableCollection<RoomType>();

            GenerateReportCommand = new RelayCommand(_ => ExecuteGenerateReport());
            ExportCommand = new RelayCommand(_ => ExecuteExport());
            RefreshCommand = new RelayCommand(_ => LoadData());
            RevenueReportCommand = new RelayCommand(_ => GenerateRevenueReport());
            BookingReportCommand = new RelayCommand(_ => GenerateBookingReport());
            RoomReportCommand = new RelayCommand(_ => GenerateRoomReport());
            CustomerReportCommand = new RelayCommand(_ => GenerateCustomerReport());

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                Bookings = new ObservableCollection<BookingReservation>(_bookingService.GetBookings());
                Rooms = new ObservableCollection<RoomInformation>(_roomService.GetRooms());
                RoomTypes = new ObservableCollection<RoomType>(_roomTypeService.GetRoomTypes());
                UpdateReportProperties();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
            }
        }

        private void UpdateReportProperties()
        {
            OnPropertyChanged(nameof(TotalRevenue));
            OnPropertyChanged(nameof(TotalBookings));
            OnPropertyChanged(nameof(ActiveBookings));
            OnPropertyChanged(nameof(CompletedBookings));
            OnPropertyChanged(nameof(CancelledBookings));
            OnPropertyChanged(nameof(OnlineBookings));
            OnPropertyChanged(nameof(OfflineBookings));
        }

        private void ExecuteGenerateReport()
        {
            try
            {
                ErrorMessage = string.Empty;

                if (StartDate > EndDate)
                {
                    ErrorMessage = "Start date must be before end date.";
                    return;
                }

                var filteredBookings = _bookingService.GetByDateRange(StartDate, EndDate);
                Bookings = new ObservableCollection<BookingReservation>(filteredBookings);
                UpdateReportProperties();

                switch (SelectedReportType)
                {
                    case "Revenue":
                        GenerateRevenueReport();
                        break;
                    case "Bookings":
                        GenerateBookingReport();
                        break;
                    case "Rooms":
                        GenerateRoomReport();
                        break;
                    case "Customers":
                        GenerateCustomerReport();
                        break;
                    default:
                        GenerateRevenueReport();
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error generating report: {ex.Message}";
            }
        }

        private void GenerateRevenueReport()
        {
            var report = $"=== REVENUE REPORT ===\n" +
                        $"Period: {StartDate:dd/MM/yyyy} to {EndDate:dd/MM/yyyy}\n" +
                        $"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n" +
                        $"SUMMARY:\n" +
                        $"Total Revenue: ${TotalRevenue:N2}\n" +
                        $"Total Bookings: {TotalBookings}\n" +
                        $"Average Revenue per Booking: ${(TotalBookings > 0 ? TotalRevenue / TotalBookings : 0):N2}\n\n" +
                        $"BOOKING STATUS BREAKDOWN:\n" +
                        $"Active Bookings: {ActiveBookings} (${Bookings?.Where(b => b.BookingStatus == 1).Sum(b => b.TotalPrice):N2})\n" +
                        $"Completed Bookings: {CompletedBookings} (${Bookings?.Where(b => b.BookingStatus == 2).Sum(b => b.TotalPrice):N2})\n" +
                        $"Cancelled Bookings: {CancelledBookings} (${Bookings?.Where(b => b.BookingStatus == 3).Sum(b => b.TotalPrice):N2})\n\n" +
                        $"BOOKING TYPE BREAKDOWN:\n" +
                        $"Online Bookings: {OnlineBookings} (${Bookings?.Where(b => b.BookingType == 1).Sum(b => b.TotalPrice):N2})\n" +
                        $"Offline Bookings: {OfflineBookings} (${Bookings?.Where(b => b.BookingType == 2).Sum(b => b.TotalPrice):N2})\n\n" +
                        $"TOP 5 HIGHEST REVENUE BOOKINGS:\n" +
                        GenerateTopBookingsReport();

            ReportContent = report;
        }

        private void GenerateBookingReport()
        {
            var report = $"=== BOOKING REPORT ===\n" +
                        $"Period: {StartDate:dd/MM/yyyy} to {EndDate:dd/MM/yyyy}\n" +
                        $"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n" +
                        $"BOOKING STATISTICS:\n" +
                        $"Total Bookings: {TotalBookings}\n" +
                        $"Active Bookings: {ActiveBookings}\n" +
                        $"Completed Bookings: {CompletedBookings}\n" +
                        $"Cancelled Bookings: {CancelledBookings}\n" +
                        $"Online Bookings: {OnlineBookings}\n" +
                        $"Offline Bookings: {OfflineBookings}\n\n" +
                        $"AVERAGE BOOKING DURATION: {Bookings?.Average(b => b.BookingDuration):F1} days\n" +
                        $"AVERAGE BOOKING VALUE: ${(TotalBookings > 0 ? TotalRevenue / TotalBookings : 0):N2}\n\n" +
                        $"RECENT BOOKINGS:\n" +
                        GenerateRecentBookingsReport();

            ReportContent = report;
        }

        private void GenerateRoomReport()
        {
            var report = $"=== ROOM REPORT ===\n" +
                        $"Period: {StartDate:dd/MM/yyyy} to {EndDate:dd/MM/yyyy}\n" +
                        $"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n" +
                        $"ROOM STATISTICS:\n" +
                        $"Total Rooms: {Rooms?.Count ?? 0}\n" +
                        $"Active Rooms: {Rooms?.Count(r => r.RoomStatus == 1) ?? 0}\n" +
                        $"Room Types: {RoomTypes?.Count ?? 0}\n\n" +
                        $"ROOM TYPE BREAKDOWN:\n" +
                        GenerateRoomTypeBreakdown() +
                        $"\nMOST POPULAR ROOMS:\n" +
                        GeneratePopularRoomsReport();

            ReportContent = report;
        }

        private void GenerateCustomerReport()
        {
            var report = $"=== CUSTOMER REPORT ===\n" +
                        $"Period: {StartDate:dd/MM/yyyy} to {EndDate:dd/MM/yyyy}\n" +
                        $"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n" +
                        $"CUSTOMER STATISTICS:\n" +
                        $"Total Customers: {_customerService.GetCustomers().Count}\n" +
                        $"Active Customers: {_customerService.GetCustomers().Count(c => c.CustomerStatus == 1)}\n\n" +
                        $"TOP CUSTOMERS BY BOOKINGS:\n" +
                        GenerateTopCustomersReport();

            ReportContent = report;
        }

        private string GenerateTopBookingsReport()
        {
            var topBookings = Bookings?.OrderByDescending(b => b.TotalPrice).Take(5).ToList();
            if (topBookings == null || !topBookings.Any()) return "No bookings found.";

            var report = "";
            for (int i = 0; i < topBookings.Count; i++)
            {
                var booking = topBookings[i];
                report += $"{i + 1}. Booking #{booking.BookingReservationID} - ${booking.TotalPrice:N2}\n";
            }
            return report;
        }

        private string GenerateRecentBookingsReport()
        {
            var recentBookings = Bookings?.OrderByDescending(b => b.BookingDate).Take(10).ToList();
            if (recentBookings == null || !recentBookings.Any()) return "No recent bookings found.";

            var report = "";
            foreach (var booking in recentBookings)
            {
                report += $"• {booking.BookingDate:dd/MM/yyyy} - Booking #{booking.BookingReservationID} - ${booking.TotalPrice:N2}\n";
            }
            return report;
        }

        private string GenerateRoomTypeBreakdown()
        {
            var breakdown = "";
            foreach (var roomType in RoomTypes ?? new ObservableCollection<RoomType>())
            {
                var roomsOfType = Rooms?.Count(r => r.RoomTypeID == roomType.RoomTypeID) ?? 0;
                var bookingsOfType = Bookings?.Count(b => b.Room?.RoomTypeID == roomType.RoomTypeID) ?? 0;
                breakdown += $"• {roomType.RoomTypeName}: {roomsOfType} rooms, {bookingsOfType} bookings\n";
            }
            return breakdown;
        }

        private string GeneratePopularRoomsReport()
        {
            var roomBookings = Bookings?.GroupBy(b => b.RoomID)
                .Select(g => new { RoomID = g.Key, Count = g.Count(), Revenue = g.Sum(b => b.TotalPrice) })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            if (roomBookings == null || !roomBookings.Any()) return "No room bookings found.";

            var report = "";
            foreach (var roomBooking in roomBookings)
            {
                var room = Rooms?.FirstOrDefault(r => r.RoomID == roomBooking.RoomID);
                report += $"• Room {room?.RoomNumber ?? "Unknown"}: {roomBooking.Count} bookings, ${roomBooking.Revenue:N2}\n";
            }
            return report;
        }

        private string GenerateTopCustomersReport()
        {
            var customerBookings = Bookings?.GroupBy(b => b.CustomerID)
                .Select(g => new { CustomerID = g.Key, Count = g.Count(), Revenue = g.Sum(b => b.TotalPrice) })
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToList();

            if (customerBookings == null || !customerBookings.Any()) return "No customer bookings found.";

            var report = "";
            foreach (var customerBooking in customerBookings)
            {
                var customer = _customerService.GetCustomerByID(customerBooking.CustomerID);
                report += $"• {customer?.CustomerFullName ?? "Unknown"}: {customerBooking.Count} bookings, ${customerBooking.Revenue:N2}\n";
            }
            return report;
        }

        private void ExecuteExport()
        {
            try
            {
                if (string.IsNullOrEmpty(ReportContent))
                {
                    MessageBox.Show("Please generate a report first.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // For now, just show a message
                MessageBox.Show($"Report ready for export:\n\n{ReportContent.Substring(0, Math.Min(200, ReportContent.Length))}...", 
                    "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error exporting report: {ex.Message}";
            }
        }
    }
} 
