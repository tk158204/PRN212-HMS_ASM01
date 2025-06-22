using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Services
{
    public class RoomInformationService
    {
        private readonly IRoomInformationRepository _repo;

        public RoomInformationService(IRoomInformationRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }
        
        public List<RoomInformation> GetRooms()
        {
            try
            {
                return _repo.GetAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting rooms: {ex.Message}");
                throw new InvalidOperationException("Failed to retrieve rooms", ex);
            }
        }
        
        public RoomInformation? GetRoomByID(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Room ID must be greater than 0", nameof(id));
            }

            try
            {
                return _repo.GetByID(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting room by ID {id}: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve room with ID {id}", ex);
            }
        }
        
        public void AddRoom(RoomInformation room)
        {
            ValidateRoom(room);

            try
            {
                // Check if room number already exists
                var existingRoom = _repo.Search(room.RoomNumber).FirstOrDefault();
                if (existingRoom != null)
                {
                    throw new InvalidOperationException($"Room with number '{room.RoomNumber}' already exists");
                }

                _repo.Add(room);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding room: {ex.Message}");
                throw;
            }
        }
        
        public void UpdateRoom(RoomInformation room)
        {
            ValidateRoom(room);

            try
            {
                var existingRoom = _repo.GetByID(room.RoomID);
                if (existingRoom == null)
                {
                    throw new InvalidOperationException($"Room with ID {room.RoomID} not found");
                }

                // Check if room number already exists for different ID
                var duplicateRoom = _repo.Search(room.RoomNumber)
                    .FirstOrDefault(r => r.RoomID != room.RoomID);
                if (duplicateRoom != null)
                {
                    throw new InvalidOperationException($"Room with number '{room.RoomNumber}' already exists");
                }

                _repo.Update(room);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating room: {ex.Message}");
                throw;
            }
        }
        
        public void DeleteRoom(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Room ID must be greater than 0", nameof(id));
            }

            try
            {
                var room = _repo.GetByID(id);
                if (room == null)
                {
                    throw new InvalidOperationException($"Room with ID {id} not found");
                }

                _repo.Delete(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting room: {ex.Message}");
                throw;
            }
        }
        
        public List<RoomInformation> GetByRoomType(int roomTypeId)
        {
            if (roomTypeId <= 0)
            {
                throw new ArgumentException("Room type ID must be greater than 0", nameof(roomTypeId));
            }

            try
            {
                return _repo.GetByRoomType(roomTypeId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting rooms by type {roomTypeId}: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve rooms for type {roomTypeId}", ex);
            }
        }
        
        public List<RoomInformation> SearchRooms(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return GetRooms();
            }

            try
            {
                return _repo.Search(searchString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching rooms: {ex.Message}");
                throw new InvalidOperationException("Failed to search rooms", ex);
            }
        }
        
        public List<RoomInformation> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate)
        {
            if (checkInDate >= checkOutDate)
            {
                throw new ArgumentException("Check-in date must be before check-out date");
            }

            if (checkInDate < DateTime.Today)
            {
                throw new ArgumentException("Check-in date cannot be in the past");
            }

            try
            {
                return _repo.GetAvailableRooms(checkInDate, checkOutDate);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting available rooms: {ex.Message}");
                throw new InvalidOperationException("Failed to retrieve available rooms", ex);
            }
        }

        private void ValidateRoom(RoomInformation room)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(room);

            if (!Validator.TryValidateObject(room, validationContext, validationResults, true))
            {
                var errors = string.Join(", ", validationResults.Select(v => v.ErrorMessage));
                throw new ValidationException($"Room validation failed: {errors}");
            }

            // Additional business logic validation
            if (room.RoomMaxCapacity <= 0 || room.RoomMaxCapacity > 10)
            {
                throw new ValidationException("Room capacity must be between 1 and 10");
            }

            if (room.RoomPricePerDate < 0)
            {
                throw new ValidationException("Room price cannot be negative");
            }
        }
    }
} 