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

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CustomerDialogViewModel(Customer customer, bool isEditMode)
        {
            _customer = customer;
            _isEditMode = isEditMode;

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

                if (string.IsNullOrWhiteSpace(Customer.Password))
                {
                    ErrorMessage = "Password is required.";
                    return;
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
    }
}