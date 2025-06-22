using BusinessObjects;
using Services;
using Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace TranTuanKietWPF.ViewModels
{
    public class RoomViewModel : ViewModelBase
    {
        private readonly RoomInformationService _roomService;
        private readonly RoomTypeService _roomTypeService;
        private ObservableCollection<RoomInformation> _rooms;
        private ObservableCollection<RoomType> _roomTypes;
        private RoomInformation? _selectedRoom;
        private RoomType? _selectedRoomType;
        private string _searchText = string.Empty;
        private string _errorMessage = string.Empty;
        private int? _selectedRoomTypeFilter;

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

        public RoomInformation? SelectedRoom
        {
            get => _selectedRoom;
            set => SetProperty(ref _selectedRoom, value);
        }

        public RoomType? SelectedRoomType
        {
            get => _selectedRoomType;
            set
            {
                SetProperty(ref _selectedRoomType, value);
                if (value != null)
                {
                    LoadRoomsByType(value.RoomTypeID);
                }
            }
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

        public int? SelectedRoomTypeFilter
        {
            get => _selectedRoomTypeFilter;
            set 
            {
                if (SetProperty(ref _selectedRoomTypeFilter, value))
                {
                    FilterRooms();
                }
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }

        public RoomViewModel()
        {
            var roomRepository = new RoomInformationRepository();
            var roomTypeRepository = new RoomTypeRepository();
            _roomService = new RoomInformationService(roomRepository);
            _roomTypeService = new RoomTypeService(roomTypeRepository);
            
            _rooms = new ObservableCollection<RoomInformation>();
            _roomTypes = new ObservableCollection<RoomType>();

            AddCommand = new RelayCommand(_ => ExecuteAdd());
            EditCommand = new RelayCommand(_ => ExecuteEdit(), _ => SelectedRoom != null);
            DeleteCommand = new RelayCommand(_ => ExecuteDelete(), _ => SelectedRoom != null);
            SearchCommand = new RelayCommand(_ => ExecuteSearch());
            RefreshCommand = new RelayCommand(_ => LoadData());

            LoadData();
            
            if (_selectedRoomTypeFilter.HasValue && _selectedRoomTypeFilter.Value > 0)
            {
                FilterRooms();
            }
        }

        private void LoadData()
        {
            try
            {
                RoomTypes = new ObservableCollection<RoomType>(_roomTypeService.GetRoomTypes());
                LoadRooms();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
            }
        }

        private void LoadRooms()
        {
            try
            {
                Rooms = new ObservableCollection<RoomInformation>(_roomService.GetRooms());
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading rooms: {ex.Message}";
            }
        }

        private void LoadRoomsByType(int roomTypeId)
        {
            try
            {
                Rooms = new ObservableCollection<RoomInformation>(_roomService.GetByRoomType(roomTypeId));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading rooms by type: {ex.Message}";
            }
        }

        private void FilterRooms()
        {
            if (_selectedRoomTypeFilter.HasValue && _selectedRoomTypeFilter.Value > 0)
            {
                var filteredRooms = _roomService.GetRooms().Where(r => r.RoomTypeID == _selectedRoomTypeFilter.Value);
                Rooms = new ObservableCollection<RoomInformation>(filteredRooms);
            }
            else if (Rooms.Count == 0) // Chỉ load lại khi chưa có dữ liệu
            {
                LoadRooms();
            }
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadRooms();
            }
            else
            {
                try
                {
                    Rooms = new ObservableCollection<RoomInformation>(_roomService.SearchRooms(SearchText));
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error searching rooms: {ex.Message}";
                }
            }
        }

        private void ExecuteAdd()
        {
            try
            {
                // Create a new room object
                var newRoom = new RoomInformation
                {
                    RoomStatus = 1,
                    RoomMaxCapacity = 2,
                    RoomPricePerDate = 100.00M
                };

                // Show dialog
                var dialog = new RoomDialog(newRoom, false);
                dialog.Owner = Application.Current.MainWindow;
                
                if (dialog.ShowDialog() == true)
                {
                    // Get the room from dialog
                    var room = dialog.ViewModel.Room;
                    
                    // Add to database
                    _roomService.AddRoom(room);
                    
                    // Refresh list
                    LoadRooms();
                    
                    MessageBox.Show("Room added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void ExecuteEdit()
        {
            if (SelectedRoom == null) return;

            try
            {
                // Create a copy for editing
                var roomToEdit = new RoomInformation
                {
                    RoomID = SelectedRoom.RoomID,
                    RoomNumber = SelectedRoom.RoomNumber,
                    RoomDescription = SelectedRoom.RoomDescription,
                    RoomMaxCapacity = SelectedRoom.RoomMaxCapacity,
                    RoomStatus = SelectedRoom.RoomStatus,
                    RoomPricePerDate = SelectedRoom.RoomPricePerDate,
                    RoomTypeID = SelectedRoom.RoomTypeID
                };

                // Show dialog
                var dialog = new RoomDialog(roomToEdit, true);
                dialog.Owner = Application.Current.MainWindow;
                
                if (dialog.ShowDialog() == true)
                {
                    // Get the room from dialog
                    var room = dialog.ViewModel.Room;
                    
                    // Update in database
                    _roomService.UpdateRoom(room);
                    
                    // Refresh list
                    LoadRooms();
                    
                    MessageBox.Show("Room updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void ExecuteDelete()
        {
            if (SelectedRoom == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete room {SelectedRoom.RoomNumber}?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _roomService.DeleteRoom(SelectedRoom.RoomID);
                    LoadRooms();
                    MessageBox.Show("Room deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error: {ex.Message}";
                }
            }
        }
    }
} 