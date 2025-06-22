using BusinessObjects;
using Services;
using Repositories;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TranTuanKietWPF.ViewModels
{
    public class CustomerDialogViewModel : ViewModelBase
    {
        private Customer _customer;
        private bool _isEditMode;
        private string _errorMessage = string.Empty;
        private string _dialogTitle = string.Empty;

        public Customer Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
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

        public CustomerDialogViewModel(Customer customer, bool isEditMode)
        {
            _customer = customer;
            _isEditMode = isEditMode;
            _dialogTitle = isEditMode ? "Edit Customer" : "Add New Customer";

            SaveCommand = new RelayCommand(_ => ExecuteSave());
            CancelCommand = new RelayCommand(_ => ExecuteCancel());
        }

        private void ExecuteSave()
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate required fields
                if (string.IsNullOrWhiteSpace(Customer.CustomerFullName))
                {
                    ErrorMessage = "Full name is required.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Customer.EmailAddress))
                {
                    ErrorMessage = "Email address is required.";
                    return;
                }

                // Get password from PasswordBox if not in edit mode or if password is being changed
                if (!IsEditMode || string.IsNullOrWhiteSpace(Customer.Password))
                {
                    // Password will be set from the dialog's code-behind
                    if (string.IsNullOrWhiteSpace(Customer.Password))
                    {
                        ErrorMessage = "Password is required.";
                        return;
                    }
                }

                if (Customer.Password.Length < 6)
                {
                    ErrorMessage = "Password must be at least 6 characters long.";
                    return;
                }

                // Validate email format
                if (!IsValidEmail(Customer.EmailAddress))
                {
                    ErrorMessage = "Invalid email format.";
                    return;
                }

                // Validate phone number if provided
                if (!string.IsNullOrWhiteSpace(Customer.Telephone) && !IsValidPhoneNumber(Customer.Telephone))
                {
                    ErrorMessage = "Invalid phone number format.";
                    return;
                }

                // Validate birthday
                if (Customer.CustomerBirthday > DateTime.Today)
                {
                    ErrorMessage = "Birthday cannot be in the future.";
                    return;
                }

                // Close dialog with success
                CloseDialog(true);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
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

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Basic phone number validation - can be enhanced based on requirements
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^[\d\s\-\+\(\)]+$");
        }

        public void SetPassword(string password)
        {
            Customer.Password = password;
        }
    }
}