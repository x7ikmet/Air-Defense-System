using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using TunaGUI.Views;
using TunaGUI.Data;
using TunaGUI.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace TunaGUI.ViewModels;

public partial class MainViewModel:ViewModelBase
{
    private PageFactory _pageFactory;
    private bool _isFirstLoad = true;

    [ObservableProperty]
    private bool sideMenuExpanded = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HomePageIsActive))]
    [NotifyPropertyChangedFor(nameof(ServicesPageIsActive))]
    [NotifyPropertyChangedFor(nameof(SettingsPageIsActive))]
    private PageViewModel ?currentPage;

    public bool HomePageIsActive => CurrentPage?.PageName == ApplicationPageNames.Home;
    public bool ServicesPageIsActive => CurrentPage?.PageName == ApplicationPageNames.Services;
    public bool SettingsPageIsActive => CurrentPage?.PageName == ApplicationPageNames.Settings;

    // Remove the parameterless constructor that's causing the problem
    // Instead, MainViewModel should always be created with PageFactory

    public MainViewModel(PageFactory pageFactory)
    {
        _pageFactory = pageFactory;
        GoToHome();
    }

    [RelayCommand]
    private void SideMenuResize()
    {
        SideMenuExpanded ^= true;
    }

    [RelayCommand]
    private void GoToHome()
    {
        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Home);
        
        // Initialize the webcam on first load
        if (_isFirstLoad && CurrentPage is HomePageViewModel homeVM && homeVM.WebcamViewModel != null)
        {
            _isFirstLoad = false;
        }
    }
    
    [RelayCommand]
    private void GoToServices()
    {
        // Store the current page to check if we're navigating away from home
        var previousPage = CurrentPage;
        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Services);
    }
    
    [RelayCommand]
    private void GoToSettings()
    {
        // Store the current page to check if we're navigating away from home
        var previousPage = CurrentPage;
        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Settings);
    }
}
