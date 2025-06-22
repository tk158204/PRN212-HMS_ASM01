using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Services
{
    public class RoomTypeService
    {
        private readonly IRoomTypeRepository _repo;

        public RoomTypeService(IRoomTypeRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }
        
        public List<RoomType> GetRoomTypes()
        {
            try
            {
                return _repo.GetAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting room types: {ex.Message}");
                throw new InvalidOperationException("Failed to retrieve room types", ex);
            }
        }
        
        public RoomType? GetRoomTypeByID(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Room type ID must be greater than 0", nameof(id));
            }

            try
            {
                return _repo.GetByID(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting room type by ID {id}: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve room type with ID {id}", ex);
            }
        }
        
        public void Add(RoomType roomType)
        {
            ValidateRoomType(roomType);

            try
            {
                // Check if room type name already exists
                var existingRoomType = _repo.Search(roomType.RoomTypeName).FirstOrDefault();
                if (existingRoomType != null)
                {
                    throw new InvalidOperationException($"Room type with name '{roomType.RoomTypeName}' already exists");
                }

                _repo.Add(roomType);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding room type: {ex.Message}");
                throw;
            }
        }
        
        public void Update(RoomType roomType)
        {
            ValidateRoomType(roomType);

            try
            {
                var existingRoomType = _repo.GetByID(roomType.RoomTypeID);
                if (existingRoomType == null)
                {
                    throw new InvalidOperationException($"Room type with ID {roomType.RoomTypeID} not found");
                }

                // Check if room type name already exists for different ID
                var duplicateRoomType = _repo.Search(roomType.RoomTypeName)
                    .FirstOrDefault(rt => rt.RoomTypeID != roomType.RoomTypeID);
                if (duplicateRoomType != null)
                {
                    throw new InvalidOperationException($"Room type with name '{roomType.RoomTypeName}' already exists");
                }

                _repo.Update(roomType);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating room type: {ex.Message}");
                throw;
            }
        }
        
        public void Delete(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Room type ID must be greater than 0", nameof(id));
            }

            try
            {
                var roomType = _repo.GetByID(id);
                if (roomType == null)
                {
                    throw new InvalidOperationException($"Room type with ID {id} not found");
                }

                _repo.Delete(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting room type: {ex.Message}");
                throw;
            }
        }
        
        public List<RoomType> Search(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return GetRoomTypes();
            }

            try
            {
                return _repo.Search(searchString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching room types: {ex.Message}");
                throw new InvalidOperationException("Failed to search room types", ex);
            }
        }

        private void ValidateRoomType(RoomType roomType)
        {
            if (roomType == null)
            {
                throw new ArgumentNullException(nameof(roomType));
            }

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(roomType);

            if (!Validator.TryValidateObject(roomType, validationContext, validationResults, true))
            {
                var errors = string.Join(", ", validationResults.Select(v => v.ErrorMessage));
                throw new ValidationException($"Room type validation failed: {errors}");
            }
        }
    }
} 