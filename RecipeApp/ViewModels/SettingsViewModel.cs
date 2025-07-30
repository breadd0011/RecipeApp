using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeApp.Models;
using RecipeApp.Services.Localization;
using RecipeApp.Utils;

namespace RecipeApp.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        [ObservableProperty] private Language _selectedLanguage;

        private readonly AppSettings _appSettings;
        public ILocalizationService L { get; }

        public SettingsViewModel(AppSettings appSettings, ILocalizationService localizationService)
        {
            _appSettings = appSettings;
            L = localizationService;

            _selectedLanguage = Languages.FirstOrDefault(l => l.Code == _appSettings.Language);
        }

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

        private void SaveLanguageToConfig(string languageCode)
        {
            var config = new
            {
                AppSettings = new { Language = languageCode }
            };

            string json = JsonSerializer.Serialize(
                config,
                new JsonSerializerOptions { WriteIndented = true }
            );

            File.WriteAllText(Constants.ConfigFile, json);
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
    }
}
