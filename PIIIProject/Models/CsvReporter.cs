using System.Data;
using System.IO;
using System.Reflection;
using System.Text;


namespace PIIIProject.Models;

/// <summary>
/// Provides functionality to write, read, and process CSV reports for orders
/// </summary>
public static class CsvReporter
{
    //File name constant for the CSV report
    private const string FileName = "order-report.csv";
    //Full path to the CSV file, located in a relative directory
    private static readonly string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);

    /// <summary>
    /// Writes order details to a CSV file
    /// </summary>
    public static void WriteOrderReportToCsv()
    {
        Order order = new();
        StringBuilder csvContent = new StringBuilder();

        //Check if the file exists; if not, create a header line
        if (!File.Exists(fullPath))
        {
            string headerLine = "OrderNumber,OrderDate,OrderTime,TotalItems,ItemName,ItemQuantity";
            csvContent.AppendLine(headerLine);
        }

        // Set the current order number
        order.SetOrderNumber();

        // Append each order item to the CSV content
        foreach (OrderItem item in Order.GetOrderItems())
        {
            string line = $"{order.GetOrderNumber()},{order.ToCsv()},{item.ToCsv()}";
            csvContent.AppendLine(line);
        }

        //Write the CSV content to file
        try
        {
            using (StreamWriter writer = new StreamWriter(fullPath, true))
            {
                writer.Write(csvContent.ToString());
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to write report to CSV: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Retrieves the next order number by finding the maximum order number in the CSV file
    /// </summary>
    /// <returns>The next order number</returns>
    public static int GetNextOrderNumber()
    {
        int maxOrderNumber = 0;

        try
        {
            //Read the CSV file, if it exists
            if (File.Exists(fullPath))
            {
                string[] lines = File.ReadAllLines(fullPath);

                //Parse each line to find the maximum order number
                foreach (string line in lines.Skip(1)) // Skip header line
                {
                    string[] parts = line.Split(',');
                    if (parts.Length > 0 && int.TryParse(parts[0], out int currentOrderNumber))
                    {
                        maxOrderNumber = Math.Max(maxOrderNumber, currentOrderNumber);
                    }
                }
            }

            return maxOrderNumber + 1;
        }
        catch (Exception e)
        {
            throw new Exception($"Error: {e.Message}", e);
        }
    }

    /// <summary>
    /// Read the CSV file and converts it into a list of OrderDisplayModel objects
    /// </summary>
    /// <param name="start">Filter start date (optional)</param>
    /// <param name="end">Filter end date (optional)</param>
    /// <returns>List of OrderDisplayModel objects</returns>
    public static List<OrderDisplayModel> GetReportFromCsv(DateTime? start, DateTime? end)
    {
        List<OrderDisplayModel> groupedOrders = new List<OrderDisplayModel>();
        DateOnly? startDate = start.HasValue ? DateOnly.FromDateTime(start.Value) : null;
        DateOnly? endDate = end.HasValue ? DateOnly.FromDateTime(end.Value) : null;


        //Read the CSV file, if it exists
        if (File.Exists(fullPath))
        {
            IEnumerable<string> lines = File.ReadAllLines(fullPath).Skip(1); // Skip header line

            // Parse each line to create OrderDisplayModel objects.
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length > 0 && int.TryParse(parts[0], out int orderNumber))
                {
                    DateOnly orderDate = DateOnly.Parse(parts[1]);
                    TimeOnly orderTime = TimeOnly.Parse(parts[2]);
                    int totalItems = int.Parse(parts[3]);
                    string itemName = parts[4];
                    int itemQuantity = int.Parse(parts[5]);

                    groupedOrders.Add(new OrderDisplayModel(orderNumber, orderDate, orderTime, totalItems, itemName, itemQuantity));

                }
            }
        }

        return groupedOrders;
    }

    /// <summary>
    /// Converts a list of OrderDisplayModel objects into a DataTable, filtered by specified fields.
    /// </summary>
    /// <param name="groupedOrders">List of OrderDisplayModel objects.</param>
    /// <param name="fieldsList">Comma-separated string of fields to include in the DataTable.</param>
    /// <returns>A DataTable containing the specified fields of the OrderDisplayModel objects.</returns>
    public static DataTable FilterFieldsToDataTable(List<OrderDisplayModel> groupedOrders, string fieldsList)
    {
        HashSet<string> fields = fieldsList.Split(',')
            .Select(f => f.Replace(" ", ""))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        DataTable table = new DataTable();

        // Add columns to DataTable based on selected fields
        foreach (string field in fields)
        {
            table.Columns.Add(field);
        }

        // Populate the DataTable with values from groupedOrders
        foreach (OrderDisplayModel order in groupedOrders)
        {
            DataRow row = table.NewRow();
            foreach (DataColumn column in table.Columns)
            {
                PropertyInfo? prop = typeof(OrderDisplayModel).GetProperty(column.ColumnName);
                if (prop != null)
                {
                    row[column.ColumnName] = prop.GetValue(order) ?? DBNull.Value;
                }
            }
            table.Rows.Add(row);
        }

        return table;
    }

}

