using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _repo;

        public CustomerService(ICustomerRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }
        
        public List<Customer> GetCustomers()
        {
            try
            {
                return _repo.GetAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting customers: {ex.Message}");
                throw new InvalidOperationException("Failed to retrieve customers", ex);
            }
        }
        
        public Customer? GetCustomerByID(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Customer ID must be greater than 0", nameof(id));
            }

            try
            {
                return _repo.GetByID(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting customer by ID {id}: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve customer with ID {id}", ex);
            }
        }
        
        public Customer? GetCustomerByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }

            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format", nameof(email));
            }

            try
            {
                return _repo.GetByEmail(email);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting customer by email {email}: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve customer with email {email}", ex);
            }
        }
        
        public void AddCustomer(Customer customer)
        {
            ValidateCustomer(customer);

            try
            {
                // Check if email already exists
                var existingCustomer = _repo.GetByEmail(customer.EmailAddress);
                if (existingCustomer != null)
                {
                    throw new InvalidOperationException($"Customer with email '{customer.EmailAddress}' already exists");
                }

                _repo.Add(customer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding customer: {ex.Message}");
                throw;
            }
        }
        
        public void UpdateCustomer(Customer customer)
        {
            ValidateCustomer(customer);

            try
            {
                var existingCustomer = _repo.GetByID(customer.CustomerID);
                if (existingCustomer == null)
                {
                    throw new InvalidOperationException($"Customer with ID {customer.CustomerID} not found");
                }

                // Check if email already exists for different customer
                var duplicateCustomer = _repo.GetByEmail(customer.EmailAddress);
                if (duplicateCustomer != null && duplicateCustomer.CustomerID != customer.CustomerID)
                {
                    throw new InvalidOperationException($"Customer with email '{customer.EmailAddress}' already exists");
                }

                _repo.Update(customer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating customer: {ex.Message}");
                throw;
            }
        }
        
        public void DeleteCustomer(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Customer ID must be greater than 0", nameof(id));
            }

            try
            {
                var customer = _repo.GetByID(id);
                if (customer == null)
                {
                    throw new InvalidOperationException($"Customer with ID {id} not found");
                }

                _repo.Delete(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting customer: {ex.Message}");
                throw;
            }
        }
        
        public Customer? Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }

            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format", nameof(email));
            }

            try
            {
                return _repo.Login(email, password);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during login for email {email}: {ex.Message}");
                throw new InvalidOperationException("Login failed", ex);
            }
        }
        
        public List<Customer> SearchCustomers(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return GetCustomers();
            }

            try
            {
                return _repo.Search(searchString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching customers: {ex.Message}");
                throw new InvalidOperationException("Failed to search customers", ex);
            }
        }

        private void ValidateCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(customer);

            if (!Validator.TryValidateObject(customer, validationContext, validationResults, true))
            {
                var errors = string.Join(", ", validationResults.Select(v => v.ErrorMessage));
                throw new ValidationException($"Customer validation failed: {errors}");
            }

            // Additional business logic validation
            if (!IsValidEmail(customer.EmailAddress))
            {
                throw new ValidationException("Invalid email format");
            }

            if (customer.CustomerBirthday > DateTime.Today)
            {
                throw new ValidationException("Customer birthday cannot be in the future");
            }

            if (customer.CustomerBirthday < DateTime.Today.AddYears(-120))
            {
                throw new ValidationException("Customer birthday seems invalid (too old)");
            }

            if (!string.IsNullOrWhiteSpace(customer.Telephone) && !IsValidPhoneNumber(customer.Telephone))
            {
                throw new ValidationException("Invalid phone number format");
            }

            if (string.IsNullOrWhiteSpace(customer.Password) || customer.Password.Length < 6)
            {
                throw new ValidationException("Password must be at least 6 characters long");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            try
            {
                var phoneRegex = new Regex(@"^[\d\s\-\+\(\)]+$");
                return phoneRegex.IsMatch(phoneNumber) && phoneNumber.Length >= 10;
            }
            catch
            {
                return false;
            }
        }
    }
}