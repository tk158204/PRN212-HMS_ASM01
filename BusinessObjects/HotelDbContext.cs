using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BusinessObjects
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext()
        {
        }

        public HotelDbContext(DbContextOptions<HotelDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RoomType> RoomTypes { get; set; } = null!;
        public virtual DbSet<RoomInformation> RoomInformations { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<BookingReservation> BookingReservations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                try
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    IConfigurationRoot configuration = builder.Build();
                    
                    var connectionString = configuration.GetConnectionString("HotelDB");
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        // Fallback connection string
                        connectionString = "Server=localhost;Database=HotelManagementDB;User Id=SA;Password=123456;TrustServerCertificate=True;MultipleActiveResultSets=True";
                        System.Diagnostics.Debug.WriteLine("Using fallback connection string");
                    }
                    
                    optionsBuilder.UseSqlServer(connectionString);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error configuring DbContext: {ex.Message}");
                    // Use fallback connection string
                    var fallbackConnectionString = "Server=localhost;Database=HotelManagementDB;User Id=SA;Password=123456;TrustServerCertificate=True;MultipleActiveResultSets=True";
                    optionsBuilder.UseSqlServer(fallbackConnectionString);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure RoomType entity
            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.HasKey(e => e.RoomTypeID);
                entity.Property(e => e.RoomTypeID).UseIdentityColumn();
                entity.Property(e => e.RoomTypeName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TypeDescription).HasMaxLength(200);
                entity.Property(e => e.TypeNote).HasMaxLength(200);
            });

            // Configure RoomInformation entity
            modelBuilder.Entity<RoomInformation>(entity =>
            {
                entity.HasKey(e => e.RoomID);
                entity.Property(e => e.RoomID).UseIdentityColumn();
                entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.RoomDescription).HasMaxLength(200);
                entity.Property(e => e.RoomPricePerDate).HasColumnType("decimal(18,2)");
                entity.Property(e => e.RoomMaxCapacity).IsRequired();
                entity.Property(e => e.RoomStatus).IsRequired();
                
                // Define relationship with RoomType
                entity.HasOne(d => d.RoomType)
                      .WithMany()
                      .HasForeignKey(d => d.RoomTypeID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerID);
                entity.Property(e => e.CustomerID).UseIdentityColumn();
                entity.Property(e => e.CustomerFullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Telephone).HasMaxLength(20);
                entity.Property(e => e.EmailAddress).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CustomerBirthday).IsRequired();
                entity.Property(e => e.CustomerStatus).IsRequired();
                entity.Property(e => e.Password).IsRequired().HasMaxLength(50);
            });

            // Configure BookingReservation entity
            modelBuilder.Entity<BookingReservation>(entity =>
            {
                entity.HasKey(e => e.BookingReservationID);
                entity.Property(e => e.BookingReservationID).UseIdentityColumn();
                entity.Property(e => e.BookingDate).IsRequired();
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.BookingDuration).IsRequired();
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BookingStatus).IsRequired();
                entity.Property(e => e.BookingType).IsRequired();
                
                // Define relationship with Customer
                entity.HasOne(d => d.Customer)
                      .WithMany()
                      .HasForeignKey(d => d.CustomerID)
                      .OnDelete(DeleteBehavior.Restrict);
                
                // Define relationship with RoomInformation
                entity.HasOne(d => d.Room)
                      .WithMany()
                      .HasForeignKey(d => d.RoomID)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
} 