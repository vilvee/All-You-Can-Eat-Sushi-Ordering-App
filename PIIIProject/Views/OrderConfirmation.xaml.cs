using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PIIIProject.Models;

namespace PIIIProject.Views
{
    /// <summary>
    /// Interaction logic for OrderConfirmation.xaml
    /// </summary>
    public partial class OrderConfirmation : Views.BaseWindow
    {
        private MainWindow mainWindow; // Reference to the main window

        public OrderConfirmation(MainWindow mainWindow)
        {
            InitializeComponent(); // Initialize component method to set up the window
            PopulateOrderDetails(); // Populate the order details on initialization
            this.mainWindow = mainWindow; // Set the main window reference
        }

        private void PopulateOrderDetails()
        {
            // Method to populate the order details in the window
            CheckOutItemsList.ItemsSource = Order.GetOrderItems(); // Set the items source for the checkout list
            TotalItems.Text = Order.GetTotalItemCount().ToString(); // Display the total item count
        }

        private void BtnConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            // Handles the 'Confirm Order' button click event
            if (CheckOutItemsList.Items.Count == 0)
            {
                // Checks if there are items in the order
                MessageBox.Show("Please add items to your order before confirming");
                return; // Exits the method if no items are in the order
            }

            CsvReporter.WriteOrderReportToCsv(); // Generates a CSV report for the order
            MessageBox.Show("Order Confirmed"); // Shows confirmation message
            Order.StartNewOrder(); // Starts a new order
            mainWindow.RefreshDataGrid(); // Refreshes the data grid in the main window
            Close(); // Closes the current window
        }
    }
}

