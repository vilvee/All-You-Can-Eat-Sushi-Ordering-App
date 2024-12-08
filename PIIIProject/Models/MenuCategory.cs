namespace PIIIProject.Models;

public class MenuCategory
{
    public string Name { get; set; }
    public List<MenuItem> MenuItems { get; set; }

    public MenuCategory(string name) 
    {
        Name = name;
        MenuItems = new List<MenuItem>();
    }
}