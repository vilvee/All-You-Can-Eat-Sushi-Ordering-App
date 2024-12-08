using System.Windows;
using System.Windows.Input;

namespace PIIIProject.Views;

public class BaseWindow : Window
{
    public BaseWindow()
    {
        // Set up your base window properties here
        this.Height = 450;
        this.Width = 800;
        this.Title = "BaseWindow";
        // ... other properties and event handlers ...
    }

    protected void BtnMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized; // Minimizes the window
    }

    protected void BtnMaximize_Click(object sender, RoutedEventArgs e)
    {
        // Toggles between maximized and normal state
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    protected void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close(); // Closes the window
    }

    protected void TitlebarDragMaximize_LeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Handles window drag and double-click maximize behavior
        if (e.ClickCount == 2)
        {
            // Double-click toggles between maximized and normal state
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
        else if (e.LeftButton == MouseButtonState.Pressed)
        {
            // Dragging logic for a maximized window
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;

                double pct = PointToScreen(e.GetPosition(this)).X / SystemParameters.PrimaryScreenWidth;
                Top = 0;
                Left = e.GetPosition(this).X - (pct * Width);
            }

            DragMove(); // Enables the window to be dragged
        }
    }
}