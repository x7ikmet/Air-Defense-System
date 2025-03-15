using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using TunaGUI.ViewModels;

namespace TunaGUI.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void Button_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount%2 != 0) return;

        (DataContext as MainViewModel)?.SideMenuResizeCommand.Execute(this);
    }
}