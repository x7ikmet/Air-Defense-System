using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using TunaGUI.Views;
using TunaGUI.Data;
using TunaGUI.Factories;

namespace TunaGUI.ViewModels;

public partial class MainViewModel:ViewModelBase
{
    private PageFactory _pageFactory;

    [ObservableProperty]
    private bool sideMenuExpanded = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HomePageIsActive))]
    [NotifyPropertyChangedFor(nameof(ServicesPageIsActive))]
    [NotifyPropertyChangedFor(nameof(SettingsPageIsActive))]
    private PageViewModel ?currentPage;

    public bool HomePageIsActive => CurrentPage.PageName == ApplicationPageNames.Home;
    public bool ServicesPageIsActive => CurrentPage.PageName == ApplicationPageNames.Services;
    public bool SettingsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Settings;




    public MainViewModel()
    {
        CurrentPage = new HomePageViewModel();
    }
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
    }
    [RelayCommand]
    private void GoToServices()
    {
        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Services);
    }
    [RelayCommand]
    private void GoToSettings()
    {
        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Settings);
    }
}
