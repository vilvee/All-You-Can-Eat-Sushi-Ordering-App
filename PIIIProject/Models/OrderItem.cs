

namespace PIIIProject.Models
{
    /// <summary>
    /// Represents an item in an order, inheriting from MenuItem.
    /// </summary>
    public class OrderItem : MenuItem
    {
        private int _quantity;

        /// <summary>
        /// Gets or sets the quantity of the order item. 
        /// Throws an exception if the value is negative.
        /// </summary>
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Quantity cannot be negative");
                _quantity = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuItem">The MenuItem this OrderItem is based on.</param>
        /// <param name="quantity">The quantity of the MenuItem.</param>
        public OrderItem(MenuItem menuItem, int quantity)
            : base(menuItem.Name, menuItem.ImageFileName, menuItem.Description, menuItem.Category)
        {
            Quantity = quantity;
        }

        /// <summary>
        /// Converts the order item details to a CSV format string.
        /// </summary>
        /// <returns>A string representing the order item in CSV format, or null if the item name is null.</returns>
        public string? ToCsv()
        {
            return $"{Name},{Quantity}";
        }

    }
}
