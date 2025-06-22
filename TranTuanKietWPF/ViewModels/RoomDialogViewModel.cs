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

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public RoomDialogViewModel(RoomInformation room, bool isEditMode)
        {
            var roomTypeRepository = new RoomTypeRepository();
            _roomTypeService = new RoomTypeService(roomTypeRepository);
            
            _room = room;
            _isEditMode = isEditMode;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading room types: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSave()
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(Room.RoomNumber))
                {
                    MessageBox.Show("Room number is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Room.RoomTypeID <= 0)
                {
                    MessageBox.Show("Please select a room type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Room.RoomMaxCapacity <= 0)
                {
                    MessageBox.Show("Room capacity must be greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Room.RoomPricePerDate < 0)
                {
                    MessageBox.Show("Room price cannot be negative.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Close dialog with success
                CloseDialog(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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