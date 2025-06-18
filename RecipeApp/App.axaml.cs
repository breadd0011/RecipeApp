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
using RecipeApp.Services.Localization;
using RecipeApp.Services.Navigation;
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
            collection.AddSingleton<MainWindowViewModel>();
            collection.AddSingleton<RecipeExplorerViewModel>();
            collection.AddSingleton<FavoritesViewModel>();
            collection.AddTransient<AddRecipeViewModel>();
            collection.AddTransient<OpenedRecipeViewModel>();

            collection.AddSingleton<ILocalizationService, LocalizationService>();
            collection.AddSingleton<IRecipeDataService, RecipeDataService>();
            collection.AddSingleton<INavigationService, NavigationService>();
            collection.AddSingleton<ISearchService, SearchService>();

            collection.AddSingleton<Func<Type, ViewModelBase>>(serviceProvider => viewModelType => (ViewModelBase)serviceProvider.GetRequiredService(viewModelType));

            collection.AddDbContext<RecipeDbContext>();

            var serviceProvider = collection.BuildServiceProvider();

            EnsureConfigFileExists();
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("settings.json")
            .Build();

            var defaultLang = config["AppSettings:DefaultLanguage"] ?? "en";
            Thread.CurrentThread.CurrentCulture = new CultureInfo(defaultLang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(defaultLang);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
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

        private void EnsureConfigFileExists()
        {
            if (!File.Exists(Constants.ConfigFile))
            {
                var defaultConfig = new
                {
                    AppSettings = new { DefaultLanguage = "en" }
                };

                File.WriteAllText(
                    Constants.ConfigFile,
                    JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true })
                );
            }
        }
    }
}