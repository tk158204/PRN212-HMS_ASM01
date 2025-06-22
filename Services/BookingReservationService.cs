using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Services
{
    public class BookingReservationService
    {
        private readonly IBookingReservationRepository _repo;

        public BookingReservationService(IBookingReservationRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }
        
        public List<BookingReservation> GetBookings()
        {
            try
            {
                return _repo.GetAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting bookings: {ex.Message}");
                throw new InvalidOperationException("Failed to retrieve bookings", ex);
            }
        }
        
        public BookingReservation? GetBookingByID(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Booking ID must be greater than 0", nameof(id));
            }

            try
            {
                return _repo.GetByID(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting booking by ID {id}: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve booking with ID {id}", ex);
            }
        }
        
        public void Add(BookingReservation booking)
        {
            ValidateBooking(booking);

            try
            {
                // Check for booking conflicts
                var conflictingBookings = _repo.GetByRoom(booking.RoomID)
                    .Where(b => b.BookingStatus == 1 && // Active bookings only
                               b.BookingReservationID != booking.BookingReservationID &&
                               ((booking.StartDate >= b.StartDate && booking.StartDate < b.EndDate) ||
                                (booking.EndDate > b.StartDate && booking.EndDate <= b.EndDate) ||
                                (booking.StartDate <= b.StartDate && booking.EndDate >= b.EndDate)))
                    .ToList();

                if (conflictingBookings.Any())
                {
                    throw new InvalidOperationException("The selected room is not available for the specified dates");
                }

                _repo.Add(booking);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding booking: {ex.Message}");
                throw;
            }
        }
        
        public void Update(BookingReservation booking)
        {
            ValidateBooking(booking);

            try
            {
                var existingBooking = _repo.GetByID(booking.BookingReservationID);
                if (existingBooking == null)
                {
                    throw new InvalidOperationException($"Booking with ID {booking.BookingReservationID} not found");
                }

                // Check for booking conflicts (excluding current booking)
                var conflictingBookings = _repo.GetByRoom(booking.RoomID)
                    .Where(b => b.BookingStatus == 1 && // Active bookings only
                               b.BookingReservationID != booking.BookingReservationID &&
                               ((booking.StartDate >= b.StartDate && booking.StartDate < b.EndDate) ||
                                (booking.EndDate > b.StartDate && booking.EndDate <= b.EndDate) ||
                                (booking.StartDate <= b.StartDate && booking.EndDate >= b.EndDate)))
                    .ToList();

                if (conflictingBookings.Any())
                {
                    throw new InvalidOperationException("The selected room is not available for the specified dates");
                }

                _repo.Update(booking);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating booking: {ex.Message}");
                throw;
            }
        }
        
        public void Delete(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Booking ID must be greater than 0", nameof(id));
            }

            try
            {
                var booking = _repo.GetByID(id);
                if (booking == null)
                {
                    throw new InvalidOperationException($"Booking with ID {id} not found");
                }

                _repo.Delete(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting booking: {ex.Message}");
                throw;
            }
        }
        
        public List<BookingReservation> GetByCustomer(int customerId)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException("Customer ID must be greater than 0", nameof(customerId));
            }

            try
            {
                return _repo.GetByCustomer(customerId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting bookings by customer {customerId}: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve bookings for customer {customerId}", ex);
            }
        }
        
        public List<BookingReservation> GetByRoom(int roomId)
        {
            if (roomId <= 0)
            {
                throw new ArgumentException("Room ID must be greater than 0", nameof(roomId));
            }

            try
            {
                return _repo.GetByRoom(roomId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting bookings by room {roomId}: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve bookings for room {roomId}", ex);
            }
        }
        
        public List<BookingReservation> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be before or equal to end date");
            }

            try
            {
                return _repo.GetByDateRange(startDate, endDate);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting bookings by date range: {ex.Message}");
                throw new InvalidOperationException("Failed to retrieve bookings for the specified date range", ex);
            }
        }
            
        public List<BookingReservation> GetByBookingType(int bookingType)
        {
            if (bookingType < 1 || bookingType > 2)
            {
                throw new ArgumentException("Booking type must be 1 (Online) or 2 (Offline)", nameof(bookingType));
            }

            try
            {
                return _repo.GetByBookingType(bookingType);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting bookings by type {bookingType}: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve bookings for type {bookingType}", ex);
            }
        }

        private void ValidateBooking(BookingReservation booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking));
            }

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(booking);

            if (!Validator.TryValidateObject(booking, validationContext, validationResults, true))
            {
                var errors = string.Join(", ", validationResults.Select(v => v.ErrorMessage));
                throw new ValidationException($"Booking validation failed: {errors}");
            }

            // Additional business logic validation
            if (booking.StartDate >= booking.EndDate)
            {
                throw new ValidationException("Start date must be before end date");
            }

            if (booking.StartDate < DateTime.Today)
            {
                throw new ValidationException("Start date cannot be in the past");
            }

            if (booking.BookingDuration <= 0 || booking.BookingDuration > 365)
            {
                throw new ValidationException("Booking duration must be between 1 and 365 days");
            }

            if (booking.TotalPrice < 0)
            {
                throw new ValidationException("Total price cannot be negative");
            }

            if (booking.BookingStatus < 1 || booking.BookingStatus > 3)
            {
                throw new ValidationException("Invalid booking status");
            }

            if (booking.BookingType < 1 || booking.BookingType > 2)
            {
                throw new ValidationException("Invalid booking type");
            }

            // Validate that booking duration matches date range
            var expectedDuration = (int)(booking.EndDate - booking.StartDate).TotalDays;
            if (booking.BookingDuration != expectedDuration)
            {
                throw new ValidationException("Booking duration does not match the date range");
            }
        }
    }
} 