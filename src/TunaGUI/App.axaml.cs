using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using TunaGUI.ViewModels;
using TunaGUI.Views;
using Microsoft.Extensions.DependencyInjection;
using TunaGUI.Factories;
using System;
using TunaGUI.Data;
using TunaGUI.Services;

namespace TunaGUI;

public partial class App : Application
{
    // Add a static ServiceProvider to access services from anywhere
    public static IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        
        // Register services
        collection.AddSingleton<IWebcamService, WebcamService>();
        
        // Register ViewModels - Keep webcam and home page as singletons
        collection.AddSingleton<MainViewModel>();
        collection.AddSingleton<WebcamViewModel>();  // Changed to singleton
        collection.AddSingleton<HomePageViewModel>(); // Changed to singleton
        collection.AddTransient<ServicesPageViewModel>();
        collection.AddTransient<SettingsPageViewModel>();

        collection.AddSingleton<Func<ApplicationPageNames, PageViewModel>>(x => name => name switch
        {
            ApplicationPageNames.Home => x.GetRequiredService<HomePageViewModel>(),
            ApplicationPageNames.Services => x.GetRequiredService<ServicesPageViewModel>(),
            ApplicationPageNames.Settings => x.GetRequiredService<SettingsPageViewModel>(),
            _ => throw new InvalidOperationException()
        });

        collection.AddSingleton<PageFactory>();

        Services = collection.BuildServiceProvider();


        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}