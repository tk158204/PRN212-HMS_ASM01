using BusinessObjects;
using Services;
using Repositories;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace TranTuanKietWPF.ViewModels
{
    public class RoomDialogViewModel : ViewModelBase
    {
        private readonly RoomTypeService _roomTypeService;
        private RoomInformation _room;
        private ObservableCollection<RoomType> _roomTypes;
        private bool _isEditMode;
        private string _errorMessage = string.Empty;
        private string _dialogTitle = string.Empty;

        public RoomInformation Room
        {
            get => _room;
            set => SetProperty(ref _room, value);
        }

        public ObservableCollection<RoomType> RoomTypes
        {
            get => _roomTypes;
            set => SetProperty(ref _roomTypes, value);
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

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public RoomDialogViewModel(RoomInformation room, bool isEditMode)
        {
            var roomTypeRepository = new RoomTypeRepository();
            _roomTypeService = new RoomTypeService(roomTypeRepository);
            
            _room = room;
            _isEditMode = isEditMode;
            _dialogTitle = isEditMode ? "Edit Room" : "Add New Room";
            _roomTypes = new ObservableCollection<RoomType>();

            SaveCommand = new RelayCommand(_ => ExecuteSave());
            CancelCommand = new RelayCommand(_ => ExecuteCancel());

            LoadRoomTypes();
        }

        private void LoadRoomTypes()
        {
            try
            {
                RoomTypes = new ObservableCollection<RoomType>(_roomTypeService.GetRoomTypes());
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading room types: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"LoadRoomTypes error: {ex}");
            }
        }

        private void ExecuteSave()
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate required fields
                if (string.IsNullOrWhiteSpace(Room.RoomNumber))
                {
                    ErrorMessage = "Room number is required.";
                    return;
                }

                if (Room.RoomTypeID <= 0)
                {
                    ErrorMessage = "Please select a room type.";
                    return;
                }

                if (Room.RoomMaxCapacity <= 0)
                {
                    ErrorMessage = "Room capacity must be greater than 0.";
                    return;
                }

                if (Room.RoomMaxCapacity > 10)
                {
                    ErrorMessage = "Room capacity cannot exceed 10 persons.";
                    return;
                }

                if (Room.RoomPricePerDate < 0)
                {
                    ErrorMessage = "Room price cannot be negative.";
                    return;
                }

                if (Room.RoomPricePerDate > 10000)
                {
                    ErrorMessage = "Room price seems too high. Please verify.";
                    return;
                }

                // Validate room number format
                if (!IsValidRoomNumber(Room.RoomNumber))
                {
                    ErrorMessage = "Room number should contain only letters, numbers, and hyphens.";
                    return;
                }

                // Close dialog with success
                CloseDialog(true);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving room: {ex.Message}";
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

        private bool IsValidRoomNumber(string roomNumber)
        {
            // Basic room number validation - alphanumeric and hyphens allowed
            return System.Text.RegularExpressions.Regex.IsMatch(roomNumber, @"^[a-zA-Z0-9\-]+$");
        }
    }
} 