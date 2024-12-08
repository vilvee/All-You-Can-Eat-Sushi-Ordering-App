using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using PIIIProject.Models;
using PIIIProject.Views;
using MenuItem = PIIIProject.Models.MenuItem;


namespace PIIIProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Views.BaseWindow, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent(); // Initialize component method to set up the window
            InitializeMenuItems(); // Display the menu items when the window loads
            GetCategoryGroup(); // Load categories in ListView
            LoadCategories();    // Load the categories when the window loads
            DataContext = new Order();

        }

        private CollectionView GetCategoryGroup()
        {
            // Load categories 
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(MenuItemsDisplay.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Category");

            // Check if the group description is already added to avoid duplication
            bool alreadyExists = false;
            foreach (GroupDescription gd in view.GroupDescriptions)
            {
                if (gd is PropertyGroupDescription { PropertyName: "Category" })
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
            {
                view.GroupDescriptions.Add(groupDescription);
            }

            return view;
        }


        private void LoadCategories()
        {
            CategoryTabs.ItemsSource = MenuItem.Categories;
        }


        private void InitializeMenuItems()
        {
            MenuItem.LoadMenuItems(); // Load menu items from the source
            MenuItemsDisplay.ItemsSource = MenuItem.LoadedMenuItems; // Set the source for menu items display
        }

        public void RefreshDataGrid()
        {
            // Refreshes the DataGrid that displays selected items
            SelectedItemsList.ItemsSource = Order.GetOrderItems(); // Set the source for selected items
            SelectedItemsList.Items.Refresh(); // Refresh the DataGrid
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Event handler for search box text changes
            string searchText = SearchBox.Text.ToLower();
            if (!MenuItem.LoadedMenuItems.Any() || MenuItemsDisplay is null) return;

            // Filter the displayed menu items based on the search text
            if (string.IsNullOrWhiteSpace(searchText) || searchText == "search...")
            {
                MenuItemsDisplay.ItemsSource = MenuItem.LoadedMenuItems;
            }
            else
            {
                List<MenuItem> filteredItems = MenuItem.LoadedMenuItems.Where(item => item.Name.ToLower().Contains(searchText)).ToList();
                MenuItemsDisplay.ItemsSource = new ObservableCollection<MenuItem>(filteredItems);
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Clears the search box when it gains focus if the default text is present
            if (SearchBox.Text == "Search...")
            {
                SearchBox.Text = "";
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Restores the default text in the search box if it is empty when losing focus
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Search...";
            }
        }

        private void ItemDetails_Click(object sender, MouseButtonEventArgs e)
        {
            // Event handler for clicking on a menu item
            Rectangle rectangle = sender as Rectangle;

            if (rectangle.DataContext is MenuItem menuItem)
            {
                MenuItemDetails detailWindow = new MenuItemDetails(this) { Owner = this };
                detailWindow.DataContext = menuItem;
                detailWindow.ShowDialog(); // Open the details window for the selected menu item
            }
        }

     

        private void BtnSendOrder_Click(object sender, RoutedEventArgs e)
        {
            // Event handler for the 'Send Order' button click
            OrderConfirmation detailWindow = new OrderConfirmation(this) { Owner = this };
            detailWindow.ShowDialog(); // Open the order confirmation window
        }

        private void ChangeSelection_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Handler for double-clicking on a selected item in the DataGrid
            if (SelectedItemsList.SelectedItem is OrderItem selectedItem)
            {
                MenuItemDetails detailWindow = new MenuItemDetails(this)
                {
                    Owner = this,
                    DataContext = selectedItem
                };

                detailWindow.ShowDialog(); // Open the details window for the selected item
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Handler for mouse click events on the Grid
            // Deselects the item in DataGrid if the click is outside the DataGrid
            if (e.OriginalSource is FrameworkElement source && !(source.Parent is DataGrid))
            {
                MainGrid.Focus();
                SelectedItemsList.SelectedItem = null;
            }
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            // Event handler for the 'Admin' button click
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show(); // Opens the Admin window
            Close(); // Closes the current main window
        }

        private static FrameworkElement FindGroupContainer(ListView menuItemsDisplay, Type tp, string category)
        {
            // Get the default view of the ListView's items.
            CollectionView groupedView = (CollectionView)CollectionViewSource.GetDefaultView(menuItemsDisplay.ItemsSource);

            foreach (object group in groupedView.Groups)
            {
                CollectionViewGroup collection = group as CollectionViewGroup;
                if (collection.Name.ToString() == category)
                {
                    DependencyObject groupItem = menuItemsDisplay.ItemContainerGenerator.ContainerFromItem(group);
                    if (groupItem != null)
                        return FindElementInVisualTree(groupItem, tp, category);
                }
            }
            return null;
        }

        private static FrameworkElement FindElementInVisualTree(DependencyObject parent, Type targetType, string text = null)
        {
            if (parent == null) return null;

            FrameworkElement foundElement = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child.GetType() == targetType)
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        foundElement = child as FrameworkElement;
                        break;
                    }
                    else if (child is TextBlock textBlock && textBlock.Text.Equals(text, StringComparison.Ordinal))
                    {
                        foundElement = textBlock;
                        break;
                    }
                    else if (child is FrameworkElement frameworkElement && frameworkElement.Name.Equals(text, StringComparison.Ordinal))
                    {
                        foundElement = frameworkElement;
                        break;
                    }
                }

                // Recurse deeper if the child isn't the target type or doesn't contain the target text
                foundElement = FindElementInVisualTree(child, targetType, text);

                if (foundElement != null)
                {
                    break; // Found, so break
                }
            }
            return foundElement;
        }


    private void CategoryTab_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                string category = textBlock.Text;
                ScrollViewer scrollViewer = GetScrollViewer(MenuItemsDisplay);
               
                if (scrollViewer != null)
                {
                    double offset = CalculateHeaderTopOffset(MenuItemsDisplay, category);
                    
                    if (offset == 0)
                        offset = 0.2; // For some reason there is no scroll when it hits 0, so I set it to a small enough value that scrolls to the top

                    if (Math.Abs(offset) > 0.1 || offset < -1)
                             scrollViewer.ScrollToVerticalOffset(offset);
                }
            }
        }

        private double CalculateHeaderTopOffset(ListView listView, string category)
        {
            TextBlock? header = (TextBlock?)FindGroupContainer(listView, typeof(TextBlock),category);
            ScrollViewer scrollViewer = GetScrollViewer(listView);
            if (header != null && scrollViewer != null)
            {
                Point relativePoint = header.TransformToVisual(scrollViewer).Transform(new Point(0, 0));
                if (relativePoint.Y < 0)
                {
                    double newOffset = scrollViewer.VerticalOffset + relativePoint.Y;
                    newOffset = Math.Max(0, newOffset);
                    return newOffset;
                }
                return scrollViewer.VerticalOffset + relativePoint.Y;
            }
            return 0; 
        }

        private ScrollViewer GetScrollViewer(DependencyObject o)
        {
            if (o is ScrollViewer) return (ScrollViewer)o;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(o, i);
                ScrollViewer result = GetScrollViewer(child);
                return result;
            }
            return null;
        }

        private void OnMenuItemsDisplayScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(sender is ListView listView) || listView.Items.Count == 0) return;

            bool foundVisibleGroup = false;
            ScrollViewer scrollViewer = GetScrollViewer(MenuItemsDisplay);

            // Get the CollectionView from the ListView
            CollectionView groupedView = GetCategoryGroup();

            foreach (object group in groupedView.Groups)
            {
                // 'group' is typically a CollectionViewGroup, not a FrameworkElement.
                CollectionViewGroup collectionViewGroup = group as CollectionViewGroup;
                if (collectionViewGroup == null) continue;

                // Use the ItemContainerGenerator to get the container for the group.
                // Check if more than half of the group container is visible
                if (MenuItemsDisplay.ItemContainerGenerator.ContainerFromItem(group) is FrameworkElement groupContainer && IsElementVisible(groupContainer, scrollViewer))
                {
                    foundVisibleGroup = true;
                    // Find and underline the corresponding tab for this group
                    UnderlineTabForGroup(collectionViewGroup.Name.ToString());
                    break; // Stop checking once you find the first visible group
                }
            }
        }


        private void UnderlineTabForGroup(string? groupName)
        {

            // Highlight current tab 
            foreach (object item in CategoryTabs.Items)
            {
                DependencyObject container = CategoryTabs.ItemContainerGenerator.ContainerFromItem(item);
                if (container is FrameworkElement element)
                {
                    TextBlock textBlock = FindTextBlock(element);
                    if (textBlock != null)
                    {
                        textBlock.TextDecorations = (item as MenuCategory)?.Name == groupName
                            ? TextDecorations.Underline
                            : null;
                    }
                }
            }
        }

        private bool IsElementVisible(FrameworkElement element, FrameworkElement container)
        {
            // If element is not visible, return null
            if (!element.IsVisible)
                return false;

            // Calculate the container and the position and size inside the container
            Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            
            // Create a rectangle representing the entire area of the container as the new Visible area
            Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);

            // Calculate the visible area 
            Rect intersection = Rect.Intersect(rect, bounds);

            if (intersection.IsEmpty) return false;

            double visibleArea = intersection.Width * intersection.Height;
            double totalArea = bounds.Width * bounds.Height;

            // Check if more than half is visible
            return visibleArea > 0 && (visibleArea / totalArea) > 0.5;
        }

        private TextBlock FindTextBlock(DependencyObject parent)
        {
            if (parent is TextBlock tb)
                return tb;
            
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                TextBlock result = FindTextBlock(child);
                if (result != null) return result;
            }

            return null;
        }

        private void MenuToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ActualWidth > 1600)
            {
                SideBar.Visibility = Visibility.Collapsed;
                MenuToggleButton.Visibility = Visibility.Collapsed;
            }

        }
        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            DrawerHost.IsLeftDrawerOpen = false;
            if (ActualWidth > 1600)
            {
                SideBar.Visibility = Visibility.Visible;
                MenuToggleButton.Visibility = Visibility.Visible;
            }

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
