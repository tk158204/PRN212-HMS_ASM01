using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class RoomType
    {
        public int RoomTypeID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string RoomTypeName { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string TypeDescription { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string TypeNote { get; set; } = string.Empty;
    }

    public class RoomInformation
    {
        public int RoomID { get; set; }
        
        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string RoomDescription { get; set; } = string.Empty;
        
        [Range(1, 10)]
        public int RoomMaxCapacity { get; set; }
        
        [Range(1, 2)]
        public int RoomStatus { get; set; } // 1 Active, 2 Deleted
        
        [Range(0, double.MaxValue)]
        public decimal RoomPricePerDate { get; set; }
        
        public int RoomTypeID { get; set; }
        public RoomType? RoomType { get; set; }
    }

    public class Customer
    {
        public int CustomerID { get; set; }
        
        [Required]
        [StringLength(100)]
        public string CustomerFullName { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Telephone { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string EmailAddress { get; set; } = string.Empty;
        
        public DateTime CustomerBirthday { get; set; }
        
        [Range(1, 2)]
        public int CustomerStatus { get; set; } // 1 Active, 2 Deleted
        
        [Required]
        [StringLength(50)]
        public string Password { get; set; } = string.Empty;
    }

    public class BookingReservation
    {
        public int BookingReservationID { get; set; }
        
        public int CustomerID { get; set; }
        public Customer? Customer { get; set; }
        
        public int RoomID { get; set; }
        public RoomInformation? Room { get; set; }
        
        public DateTime BookingDate { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        [Range(1, 365)]
        public int BookingDuration { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }
        
        [Range(1, 3)]
        public int BookingStatus { get; set; } = 1; // 1: Active, 2: Completed, 3: Cancelled
        
        [Range(1, 2)]
        public int BookingType { get; set; } = 1; // 1: Online, 2: Offline
    }
}