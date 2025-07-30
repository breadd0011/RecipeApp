using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        public ILocalizationService L { get; }

        public MainWindowViewModel(
            INavigationService navService,
            ILocalizationService localizationService,
            ISearchService searchService)
        {
            _navService = navService;
            L = localizationService;
            _searchService = searchService;

            GoToExplorer();

            if (AutoUpdater.UpdateManager.IsInstalled)
            {
                CheckForUpdates();
            }

        }

        private void CheckForUpdates()
        {
            Task.Run(async () =>
            {
                try
                {
                    bool IsUpdateDownloaded = false;

                    await AutoUpdater.CheckForUpdatesAsync();

                    if (AutoUpdater.UpdateAvailable)
                    {
                        await AutoUpdater.DownloadUpdateAsync();
                        IsUpdateDownloaded = true;
                    }

                    Dispatcher.UIThread.Post(() =>
                    {
                        if (IsUpdateDownloaded)
                        {
                            IsUpdateAvailable = true;
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            });
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
        private void GoToSettings()
        {
            NavService.NavigateTo<SettingsViewModel>();
        }

        [RelayCommand]
        private void GoToAbout()
        {
            NavService.NavigateTo<AboutViewModel>();
        }

        [RelayCommand]
        private void AddRecipe()
        {
            NavService.NavigateTo<AddRecipeViewModel>();
        }

        [RelayCommand]
        private void UpdateApp()
        {
            AutoUpdater.UpdateAndRestartApp();
        }
    }
}
