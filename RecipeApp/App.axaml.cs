using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeApp.Data;
using RecipeApp.Services;
using RecipeApp.Services.FilePicker;
using RecipeApp.Services.Localization;
using RecipeApp.Services.Navigation;
using RecipeApp.Services.Page;
using RecipeApp.Services.Search;
using RecipeApp.Utils;
using RecipeApp.ViewModels;
using RecipeApp.Views;

namespace RecipeApp
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var collection = new ServiceCollection();

            // Ensure config file exists before building configuration
            EnsureConfigFileExists();

            // Build configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json")
                .Build();

            // Create and bind AppSettings
            var appSettings = new AppSettings();
            config.GetSection("AppSettings").Bind(appSettings);
            collection.AddSingleton(appSettings);

            // Set culture
            Thread.CurrentThread.CurrentCulture = new CultureInfo(appSettings.Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(appSettings.Language);

            // Register other services
            collection.AddSingleton<MainWindowViewModel>();
            collection.AddSingleton<RecipeExplorerViewModel>();
            collection.AddSingleton<FavoritesViewModel>();
            collection.AddSingleton<SettingsViewModel>();
            collection.AddSingleton<AboutViewModel>();
            collection.AddTransient<AddRecipeViewModel>();
            collection.AddTransient<OpenedRecipeViewModel>();

            collection.AddSingleton<ILocalizationService, LocalizationService>();
            collection.AddSingleton<IRecipeDataService, RecipeDataService>();
            collection.AddSingleton<INavigationService, NavigationService>();
            collection.AddSingleton<ISearchService, SearchService>();
            collection.AddSingleton<IFileService, FileService>();
            collection.AddSingleton<PageService>();

            collection.AddSingleton<Func<Type, ViewModelBase>>(serviceProvider => viewModelType => (ViewModelBase)serviceProvider.GetRequiredService(viewModelType));

            collection.AddDbContext<RecipeDbContext>();

            var serviceProvider = collection.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
                };

                var fileService = serviceProvider.GetRequiredService<IFileService>();
                fileService.Initialize(desktop.MainWindow);
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

        private void EnsureConfigFileExists()
        {
            if (!File.Exists(Constants.ConfigFile))
            {
                var defaultConfig = new
                {
                    AppSettings = new { Language = "en" }
                };

                File.WriteAllText(
                    Constants.ConfigFile,
                    JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true })
                );
            }
        }
    }
}