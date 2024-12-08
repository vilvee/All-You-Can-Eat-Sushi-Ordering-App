using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    /// Interaction logic for MenuItemDetails.xaml
    /// </summary>
    public partial class MenuItemDetails : Views.BaseWindow
    {
        private int quantity; // Holds the current quantity of the menu item
        private MainWindow mainWindow; // Reference to the main window

        public MenuItemDetails(MainWindow mainWindow)
        {
            InitializeComponent(); // Initialize the window components
            this.mainWindow = mainWindow; // Set the main window reference
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Handler for when the window is fully loaded
            if (DataContext is OrderItem menuItem)
            {
                // Set initial values if the DataContext is an OrderItem
                quantity = menuItem.Quantity;
                UpdateQuantityDisplay(); // Update the display for quantity
                ChangeButtonMessage(); // Change the button message based on quantity
            }
        }

        private void ChangeButtonMessage()
        {
            // Changes the text of the 'Add to Order' button based on quantity
            btnAddItem.Content = quantity == 0 ? "Add to Order" : "Update Order";
        }

        private void IncrementQuantity_Click(object sender, RoutedEventArgs e)
        {
            // Increases the quantity and updates the display
            quantity++;
            UpdateQuantityDisplay();
        }

        private void DecrementQuantity_Click(object sender, RoutedEventArgs e)
        {
            // Decreases the quantity but not below 0 and updates the display
            if (quantity > 0)
            {
                quantity--;
                UpdateQuantityDisplay();
            }
        }

        private void UpdateQuantityDisplay()
        {
            // Updates the text display for the quantity
            QuantityDisplay.Text = quantity.ToString();
        }

        private void AddToOrder_Click(object sender, RoutedEventArgs e)
        {
            // Handles the 'Add to Order' button click
            MenuItem menuItem = DataContext as MenuItem;

            if (quantity == 0)
            {
                // If quantity is 0, remove the item from the order
                Order.RemoveFromOrder(menuItem);
            }
            else
            {
                // Otherwise, add or update the item in the order with the new quantity
                Order.AddOrUpdateOrderItem(menuItem, quantity);
            }

            mainWindow.RefreshDataGrid(); // Refresh the data in the main window
            Close(); // Close the current window
        }
    }
}
