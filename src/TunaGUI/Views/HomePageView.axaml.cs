using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TunaGUI.ViewModels;
using System;

namespace TunaGUI.Views;

public partial class HomePageView : UserControl
{
    public HomePageView()
    {
        InitializeComponent();
        
        // When the control is loaded, activate the HomePageViewModel
        this.AttachedToVisualTree += (s, e) =>
        {
            if (DataContext is HomePageViewModel viewModel)
            {
                // Use Task.Run to avoid blocking the UI thread
                // Since we can't use await directly in the event handler
                _ = viewModel.OnActivated().ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        // Log any errors that occurred during activation
                        Console.WriteLine($"Error activating HomePageViewModel: {task.Exception}");
                    }
                });
            }
        };
    }
}