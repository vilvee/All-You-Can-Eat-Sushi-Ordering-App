
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace PIIIProject.Models
{
    /// <summary>
    /// Represents a menu item with properties like name, image, and description.
    /// </summary>
    public class MenuItem
    {
        //Static list to store all loaded menu items
        public static List<MenuItem> LoadedMenuItems = new();

        // Static list to store all categories
        public static List<MenuCategory> Categories = new();

        //Private field for the name of new menu item
        private readonly string _name;
        private readonly string _imageFileName;
        private readonly string _description;
        private string _category;

        /// <summary>
        /// Gets the name of the menu item. Name cannot be null or whitespace.
        /// </summary>
        public string Name => _name;
        
        /// <summary>
        /// Gets the image file name of the menu item. Cannot be null or whitespace.
        /// </summary>
        public string ImageFileName => _imageFileName;
   

        // Add a category property
        public string Category => _category;

        /// <summary>
        /// Gets the icon image of the menu item.
        /// We went with an autoproperty since, we do not want to validate the image, only set it once when it loads
        /// </summary>
        public BitmapImage IconImage { get; init; }

        /// <summary>
        /// Gets the description of the menu item. Cannot be null.
        /// </summary>
        public string Description => _description;

        /// <summary>
        /// Constructs a new instance of MenuItem with specified name, image name, and description.
        /// </summary>
        /// <param name="name">The name of the menu item.</param>
        /// <param name="imageName">The image file name of the menu item.</param>
        /// <param name="description">The description of the menu item.</param>
        public MenuItem(string name, string imageName, string description, string category)
        {
            _name = ValidateInput(name, "Name");
            _imageFileName = ValidateInput(imageName, "Image file name");
            _description = ValidateInput(description, "Description");
            _category = category;
            IconImage = new BitmapImage(new Uri($"pack://application:,,,{_imageFileName}"));
        }

        /// <summary>
        /// Validation Method
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string ValidateInput(string input, string paramName)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException($"{paramName} cannot be null or whitespace.");
            }
            return input;
        }

        /// <summary>
        /// Loads menu items into the static list from a CSV file, if not already loaded.
        /// </summary>
        public static void LoadMenuItems()
        {
            // TODO: ENCAPSULATE MORE
            if (LoadedMenuItems.Any()) return;

            // Read categories
            List<string> categoryNames = LoadCategories();
            foreach (string categoryName in categoryNames)
            {
                Categories.Add(new MenuCategory(categoryName));
            }

            // Load menu items and add them to their respective category
            foreach (MenuCategory category in Categories)
            {
                string fileName = $"{category.Name}.csv";
                LoadMenuItemsFromCsv(fileName, category);

            }
        }

        private static List<string> LoadCategories()
        {
            List<string> categories = new List<string>();
            Uri uri = new Uri("pack://application:,,,/Assets/Data/Categories.csv");
            StreamResourceInfo csvFilePath = Application.GetResourceStream(uri);

            try
            {
                using (StreamReader reader = new StreamReader(csvFilePath.Stream))
                {
                    string line = reader.ReadLine();
                    if (line != null)
                    {
                        categories.AddRange(line.Split(','));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}");
            }

            return categories;
        }

        private static void LoadMenuItemsFromCsv(string fileName, MenuCategory category)
        {
            Uri uri = new Uri($"pack://application:,,,/Assets/Data/{fileName}");
            StreamResourceInfo csvFilePath = Application.GetResourceStream(uri);

            try
            {
                using (StreamReader reader = new StreamReader(csvFilePath.Stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');

                        // Split the line into parts
                        string name = parts[0];
                        string description = parts[1].Trim('"');
                        string imageFileName = parts[2];
                        MenuItem menuItem = new MenuItem(name, imageFileName, description, category.Name);
                        LoadedMenuItems.Add(menuItem);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading menu items from {fileName}: {ex.Message}");
            }
        }
    }
}
