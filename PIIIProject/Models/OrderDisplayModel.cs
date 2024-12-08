
namespace PIIIProject.Models
{
    /// <summary>
    /// Represents a model for displaying order details.
    /// </summary>
    public class OrderDisplayModel
    {
        private string _orderDate;

        /// <summary>
        /// Gets or sets the order number. Nullable.
        /// </summary>
        public int? OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the order date. Ensures the date is formatted correctly.
        /// </summary>
        public DateOnly? OrderDate { get; set; }
        /// <summary>
        /// Gets or sets the order time. Nullable.
        /// </summary>
        public TimeOnly? OrderTime { get; set; }

        /// <summary>
        /// Gets or sets the total of the order. Nullable.
        /// </summary>
        public int? OrderTotal { get; set; }

        /// <summary>
        /// Gets or sets the name of the item in the order.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item. Nullable.
        /// </summary>
        public int? ItemQuantity { get; set; }

        public OrderDisplayModel()
        {

        }

        /// <summary>
        /// Constructs a new instance of OrderDisplayModel with specified details.
        /// </summary>
        /// <param name="orderNumber">The order number.</param>
        /// <param name="orderDate">The date of the order.</param>
        /// <param name="orderTime">The time of the order.</param>
        /// <param name="orderTotal">The total amount of the order.</param>
        /// <param name="orderItemName">The name of the item in the order.</param>
        /// <param name="orderItemQt">The quantity of the item.</param>
        public OrderDisplayModel(int orderNumber, DateOnly? orderDate, TimeOnly? orderTime, int orderTotal,
            string orderItemName, int orderItemQt)
        {
            OrderNumber = orderNumber;
            OrderDate = orderDate;
            OrderTime = orderTime;
            OrderTotal = orderTotal;
            ItemName = orderItemName;
            ItemQuantity = orderItemQt;
        }


    }
}