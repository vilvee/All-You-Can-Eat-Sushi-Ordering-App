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
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Views.BaseWindow
    {
        public AdminWindow()
        {
            InitializeComponent(); // Initialize component method to set up the window
        }

        private void btnPrintReport_Clicked(object sender, RoutedEventArgs e)
        {
            // Handles the 'Print Report' button click event
            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                ValidateDate(); // Validates the selected dates
                return;
            }

            // Processes selected fields to include in the report
            List<CheckBox> fields = stkFields.Children.OfType<CheckBox>().ToList();

            string selectedFields = "";
            foreach (CheckBox checkBox in fields)
            {
                if (checkBox.IsChecked == true)
                {
                    selectedFields += checkBox.Content.ToString() + ",";
                }
            }
            selectedFields = selectedFields.TrimEnd(',');
            Report report = new Report(this, selectedFields);
            report.ShowDialog(); // Displays the report in a new dialog

        }

        private void ValidateDate()
        {
            // Validates start and end dates for the report
            if (dpStartDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a start date");
            }
            else if (dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Please select an end date");
            }
            else if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
            {
                MessageBox.Show("Start date cannot be greater than end date");
            }
        }

        private void btnBackToMenu_Clicked(object sender, RoutedEventArgs e)
        {
            // Handles 'Back to Menu' button click event
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show(); // Shows the main window
            Close(); // Closes the current admin window
        }
    }
}

