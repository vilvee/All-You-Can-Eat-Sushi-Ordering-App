using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Views.BaseWindow
    {
        public Report(AdminWindow adminWindow, string fields)
        {
            InitializeComponent(); // Initialize component method to set up the window

            // Conditionally populate the DataGrid based on the provided fields
            if (!string.IsNullOrEmpty(fields))
            {
                // If fields are specified, filter the report data accordingly
                PopulateDataGrid(CsvReporter.FilterFieldsToDataTable(CsvReporter.GetReportFromCsv(adminWindow.dpStartDate.SelectedDate, adminWindow.dpEndDate.SelectedDate), fields));
            }
            else
            {
                // If no fields are specified, load the complete report data
                PopulateDataGrid(CsvReporter.GetReportFromCsv(adminWindow.dpStartDate.SelectedDate, adminWindow.dpEndDate.SelectedDate));
            }
        }

        public void PopulateDataGrid(List<OrderDisplayModel> groupedOrders)
        {
            // Clear existing columns and set the source for the DataGrid
            ReportGrid.Columns.Clear();
            ReportGrid.ItemsSource = groupedOrders;
        }

        public void PopulateDataGrid(DataTable groupedOrders)
        {
            // Overloaded method for DataTable source
            // Clear existing columns and set the source for the DataGrid
            ReportGrid.Columns.Clear();
            ReportGrid.ItemsSource = groupedOrders.DefaultView;
        }
    }
}

