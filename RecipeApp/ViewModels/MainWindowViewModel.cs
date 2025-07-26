using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using RecipeApp.Models;
using RecipeApp.Services.Localization;
using RecipeApp.Services.Navigation;
using RecipeApp.Services.Search;
using RecipeApp.Utils;

namespace RecipeApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private INavigationService _navService;
        [ObservableProperty] private ISearchService _searchService;
        [ObservableProperty] private Language _selectedLanguage;

        [ObservableProperty] private string _welcomeText;
        [ObservableProperty] private bool _isUpdateAvailable = false;

        private readonly IConfiguration _config;
        public ILocalizationService L { get; }

        public MainWindowViewModel(
            INavigationService navService,
            ILocalizationService localizationService,
            ISearchService searchService)
        {
            _navService = navService;
            L = localizationService;
            _searchService = searchService;

            _config = LoadConfig();

            var defaultLangCode = _config["AppSettings:DefaultLanguage"] ?? "en";
            SelectedLanguage = Languages.FirstOrDefault(l => l.Code == defaultLangCode);
            WelcomeText = _welcomeTexts[GetRandomText()];

            GoToExplorer();

            if (AutoUpdater.UpdateManager.IsInstalled)
            {
                Initialize();
            }

        }

        private void Initialize()
        {
            Task.Run(async () =>
            {
                try
                {
                    await AutoUpdater.CheckForUpdatesAsync();
                    if (AutoUpdater.UpdateAvailable)
                    {
                        IsUpdateAvailable = true;
                    }

                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                }
            });
        }

        private ObservableCollection<string> _welcomeTexts = new ObservableCollection<string>
        {
            "Főzzünk együtt!",
            "Jó étvágyat!",
            "Mi lesz ma?",
            "Receptre fel!",
            "Mit ennél?",
            "Kezdjük el!",
            "Üdv, séf!",
            "Valami újat?",
            "Recept idő!",
            "Jó főzést!",
            "Főzzünk!",
        };

        [ObservableProperty]
        private ObservableCollection<Language> _languages = new ObservableCollection<Language>
        {
            new Language
            {
                Name = "English",
                Code = "en",
                FlagPath = "avares://RecipeApp/Assets/Flags/flag_en.png"
            },

            new Language
            {
                Name = "Hungarian",
                Code = "hu",
                FlagPath = $"avares://RecipeApp/Assets/Flags/flag_hu.png"
            }
        };

        private int GetRandomText()
        {
            Random random = new Random();
            int range = _welcomeTexts.Count;

            return random.Next(0, range);
        }

        private void SaveLanguageToConfig(string languageCode)
        {
            var config = new
            {
                AppSettings = new { DefaultLanguage = languageCode }
            };

            string json = JsonSerializer.Serialize(
                config,
                new JsonSerializerOptions { WriteIndented = true }
            );

            File.WriteAllText(Constants.ConfigFile, json);
        }

        private IConfiguration LoadConfig()
        {
            return
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Constants.ConfigFile, optional: true, reloadOnChange: true)
                .Build();
        }

        [RelayCommand]
        private void GoToExplorer()
        {
            NavService.NavigateTo<RecipeExplorerViewModel>();
        }

        [RelayCommand]
        private void GoToFavorites()
        {
            NavService.NavigateTo<FavoritesViewModel>();
        }

        [RelayCommand]
        private void AddRecipe()
        {
            NavService.NavigateTo<AddRecipeViewModel>();
        }

        [RelayCommand]
        private void ChangeCulture()
        {
            if (SelectedLanguage != null)
            {
                L.ChangeCulture(SelectedLanguage.Code);
                SaveLanguageToConfig(SelectedLanguage.Code);
            }
        }

        [RelayCommand]
        private async Task UpdateApp()
        {
            await AutoUpdater.DownloadUpdateAsync();
            AutoUpdater.UpdateAndRestartApp();
        }
    }
}
