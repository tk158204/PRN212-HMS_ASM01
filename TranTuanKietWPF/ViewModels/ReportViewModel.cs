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
        private ObservableCollection<BookingReservation> _bookings;
        private ObservableCollection<RoomInformation> _rooms;
        private ObservableCollection<RoomType> _roomTypes;
        private string _selectedReportType = "Revenue";
        private DateTime _startDate = DateTime.Today.AddDays(-30);
        private DateTime _endDate = DateTime.Today;
        private string _errorMessage = string.Empty;

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

        public ICommand GenerateReportCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand RefreshCommand { get; }

        public ReportViewModel()
        {
            var bookingRepository = new BookingReservationRepository();
            var roomRepository = new RoomInformationRepository();
            var roomTypeRepository = new RoomTypeRepository();
            
            _bookingService = new BookingReservationService(bookingRepository);
            _roomService = new RoomInformationService(roomRepository);
            _roomTypeService = new RoomTypeService(roomTypeRepository);
            
            _bookings = new ObservableCollection<BookingReservation>();
            _rooms = new ObservableCollection<RoomInformation>();
            _roomTypes = new ObservableCollection<RoomType>();

            GenerateReportCommand = new RelayCommand(_ => ExecuteGenerateReport());
            ExportCommand = new RelayCommand(_ => ExecuteExport());
            RefreshCommand = new RelayCommand(_ => LoadData());

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                Bookings = new ObservableCollection<BookingReservation>(_bookingService.GetBookings());
                Rooms = new ObservableCollection<RoomInformation>(_roomService.GetRooms());
                RoomTypes = new ObservableCollection<RoomType>(_roomTypeService.GetRoomTypes());
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
            }
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

                string reportMessage = GenerateReportMessage(filteredBookings);
                MessageBox.Show(reportMessage, "Report Generated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error generating report: {ex.Message}";
            }
        }

        private string GenerateReportMessage(System.Collections.Generic.List<BookingReservation> bookings)
        {
            var totalRevenue = bookings.Sum(b => b.TotalPrice);
            var totalBookings = bookings.Count;
            var activeBookings = bookings.Count(b => b.BookingStatus == 1);
            var completedBookings = bookings.Count(b => b.BookingStatus == 2);
            var cancelledBookings = bookings.Count(b => b.BookingStatus == 3);
            var onlineBookings = bookings.Count(b => b.BookingType == 1);
            var offlineBookings = bookings.Count(b => b.BookingType == 2);

            return $"Report for {StartDate:dd/MM/yyyy} to {EndDate:dd/MM/yyyy}\n\n" +
                   $"Total Revenue: ${totalRevenue:N2}\n" +
                   $"Total Bookings: {totalBookings}\n" +
                   $"Active Bookings: {activeBookings}\n" +
                   $"Completed Bookings: {completedBookings}\n" +
                   $"Cancelled Bookings: {cancelledBookings}\n" +
                   $"Online Bookings: {onlineBookings}\n" +
                   $"Offline Bookings: {offlineBookings}";
        }

        private void ExecuteExport()
        {
            try
            {
                // For now, just show a message
                MessageBox.Show("Export functionality will be implemented in the future.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error exporting report: {ex.Message}";
            }
        }
    }
} 
