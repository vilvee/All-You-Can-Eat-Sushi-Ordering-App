
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PIIIProject.Models
{
    /// <summary>
    /// Represents an order, containing order date, time, and order items.
    /// </summary>
    public class Order : INotifyPropertyChanged
    {

        // Backing fields
        private int _orderNumber;
        private DateOnly _orderDate;
        private TimeOnly _orderTime;

        // Properties
        public int OrderNumber
        {
            get => _orderNumber;
            set => _orderNumber = value;
        }

        // Date and Time to set during printing of receipt
        private  DateOnly OrderDate {
            get => _orderDate;
            set => _orderDate = value;
        }

        private TimeOnly OrderTime
        {
            get=> _orderTime;
            set => _orderTime = value;
        }


        //Static list to store order items
        protected static ObservableCollection<OrderItem> order { get; } = new();

        /// <summary>
        /// Calculates and returns the total item count in the order.
        /// </summary>
        /// <returns>Total count of items as a string.</returns>
        public static string GetTotalItemCount() => order.Sum(item => item.Quantity).ToString();

        public int ItemsCount => order.Count();

        public Order()
        {
            order.CollectionChanged += (s, e) => OnPropertyChanged(nameof(ItemsCount));
        }

        /// <summary>
        /// Adds a new order item or updates an existing one in the order.
        /// </summary>
        /// <param name="menuItem">The menu item to add or update.</param>
        /// <param name="quantity">The quantity of the menu item.</param>
        public static void AddOrUpdateOrderItem(MenuItem menuItem, int quantity)
        {
            if (menuItem == null)
                throw new ArgumentNullException(nameof(menuItem), "MenuItem cannot be null");
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            OrderItem existingItem = order.FirstOrDefault(item => item.Equals(menuItem));
            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
            }
            else
            {
                order.Add(new OrderItem(menuItem, quantity));
            }
        }

        /// <summary>
        /// Removes a menu item from the order.
        /// </summary>
        /// <param name="menuItem">The menu item to remove.</param>
        public static void RemoveFromOrder(MenuItem menuItem)
        {
            if (menuItem == null)
                throw new ArgumentNullException(nameof(menuItem), "MenuItem cannot be null");

            OrderItem itemToRemove = order.FirstOrDefault(item => item.Equals(menuItem));
            if (itemToRemove != null)
            {
                order.Remove(itemToRemove);
            }
        }

        /// <summary>
        /// Gets a list of all order items.
        /// </summary>
        /// <returns>List of OrderItem objects.</returns>
        public static ObservableCollection<OrderItem> GetOrderItems()
        {
            return order;
        }

        /// <summary>
        /// Clears the current order and starts a new one.
        /// </summary>
        public static void StartNewOrder()
        {
            order.Clear();
        }

        // Instance methods

        /// <summary>
        /// Gets the current order number.
        /// </summary>
        /// <returns>The order number.</returns>
        public int GetOrderNumber()
        {
            return OrderNumber;
        }

        /// <summary>
        /// Sets the order number to the next available number.
        /// </summary>
        public void SetOrderNumber()
        {
            OrderNumber = CsvReporter.GetNextOrderNumber();
        }

        /// <summary>
        /// Sets the order date and time to the current moment.
        /// </summary>
        private void SetOrderDateAndTime()
        {
            OrderDate = DateOnly.FromDateTime(DateTime.Now);
            OrderTime = TimeOnly.FromDateTime(DateTime.Now);
        }

        /// <summary>
        /// Converts the order details to a CSV format string.
        /// </summary>
        /// <returns>String representing the order in CSV format.</returns>
        public string ToCsv()
        {
            SetOrderDateAndTime();
            return $"{OrderDate:yyyy-MM-dd},{OrderTime:hh\\:mm},{GetTotalItemCount()}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
