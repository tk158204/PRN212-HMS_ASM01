using BusinessObjects;
using System.Windows;

namespace TranTuanKietWPF
{
    public partial class CustomerDialog : Window
    {
        public ViewModels.CustomerDialogViewModel ViewModel { get; private set; }

        public CustomerDialog(Customer customer, bool isEditing)
        {
            InitializeComponent();
            ViewModel = new ViewModels.CustomerDialogViewModel(customer, isEditing);
            DataContext = ViewModel;
            
            // Set initial password if editing
            if (isEditing && !string.IsNullOrEmpty(customer.Password))
            {
                txtPassword.Password = customer.Password;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Set password from PasswordBox before saving
            ViewModel.SetPassword(txtPassword.Password);
            
            // Execute save command
            ViewModel.SaveCommand.Execute(null);
        }
    }
} 