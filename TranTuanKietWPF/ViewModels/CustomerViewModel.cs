using BusinessObjects;
using Services;
using Repositories;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace TranTuanKietWPF.ViewModels
{
    public class CustomerViewModel : ViewModelBase
    {
        private readonly CustomerService? _customerService;
        private ObservableCollection<Customer> _customers;
        private Customer? _selectedCustomer;
        private string _searchText = string.Empty;
        private string _errorMessage = string.Empty;

        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public Customer? SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand AddCommand { get; } = new RelayCommand(_ => { });
        public ICommand EditCommand { get; } = new RelayCommand(_ => { });
        public ICommand DeleteCommand { get; } = new RelayCommand(_ => { });
        public ICommand SearchCommand { get; } = new RelayCommand(_ => { });
        public ICommand RefreshCommand { get; } = new RelayCommand(_ => { });

        public CustomerViewModel()
        {
            try
            {
                var customerRepository = new CustomerRepository();
                _customerService = new CustomerService(customerRepository);
                _customers = new ObservableCollection<Customer>();

                // Reassign commands with proper implementations
                AddCommand = new RelayCommand(_ => ExecuteAdd());
                EditCommand = new RelayCommand(_ => ExecuteEdit(), _ => SelectedCustomer != null);
                DeleteCommand = new RelayCommand(_ => ExecuteDelete(), _ => SelectedCustomer != null);
                SearchCommand = new RelayCommand(_ => ExecuteSearch());
                RefreshCommand = new RelayCommand(_ => LoadCustomers());

                LoadCustomers();
            }
            catch (Exception ex)
            {
                _customers = new ObservableCollection<Customer>();
                ErrorMessage = $"Error initializing: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"CustomerViewModel constructor error: {ex}");
            }
        }

        private void LoadCustomers()
        {
            try
            {
                if (_customerService != null)
                {
                    var customers = _customerService.GetCustomers();
                    Customers = new ObservableCollection<Customer>(customers);
                    ErrorMessage = string.Empty;
                }
                else
                {
                    Customers = new ObservableCollection<Customer>();
                    ErrorMessage = "Service not initialized";
                }
            }
            catch (Exception ex)
            {
                Customers = new ObservableCollection<Customer>();
                ErrorMessage = $"Error loading customers: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"LoadCustomers error: {ex}");
            }
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadCustomers();
            }
            else
            {
                Customers = new ObservableCollection<Customer>(_customerService.SearchCustomers(SearchText));
            }
        }

        private void ExecuteAdd()
        {
            try
            {
                // Create a new customer object
                var newCustomer = new Customer
                {
                    CustomerBirthday = DateTime.Now.AddYears(-20),
                    CustomerStatus = 1
                };

                // Show dialog
                var dialog = new CustomerDialog(newCustomer, false);
                
                if (dialog.ShowDialog() == true)
                {
                    // Get the customer from dialog
                    var customer = dialog.ViewModel.Customer;
                    
                    // Add to database
                    _customerService?.AddCustomer(customer);
                    
                    // Refresh list
                    LoadCustomers();
                    
                    MessageBox.Show("Customer added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void ExecuteEdit()
        {
            if (SelectedCustomer == null) return;

            try
            {
                // Create a copy for editing
                var customerToEdit = new Customer
                {
                    CustomerID = SelectedCustomer.CustomerID,
                    CustomerFullName = SelectedCustomer.CustomerFullName,
                    EmailAddress = SelectedCustomer.EmailAddress,
                    Telephone = SelectedCustomer.Telephone,
                    CustomerBirthday = SelectedCustomer.CustomerBirthday,
                    CustomerStatus = SelectedCustomer.CustomerStatus,
                    Password = SelectedCustomer.Password
                };

                // Show dialog
                var dialog = new CustomerDialog(customerToEdit, true);
                
                if (dialog.ShowDialog() == true)
                {
                    // Get the customer from dialog
                    var customer = dialog.ViewModel.Customer;
                    
                    // Update in database
                    _customerService?.UpdateCustomer(customer);
                    
                    // Refresh list
                    LoadCustomers();
                    
                    MessageBox.Show("Customer updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void ExecuteDelete()
        {
            if (SelectedCustomer == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete {SelectedCustomer.CustomerFullName}?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _customerService.DeleteCustomer(SelectedCustomer.CustomerID);
                    LoadCustomers();
                    MessageBox.Show("Customer deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error: {ex.Message}";
                }
            }
        }
    }
} 