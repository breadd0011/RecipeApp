﻿using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeApp.Services.Localization;
using RecipeApp.Services.Navigation;
using RecipeApp.Services.Page;
using RecipeApp.Services.Search;
using RecipeApp.Utils;
using System;
using System.Threading.Tasks;

namespace RecipeApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private INavigationService _navService;
        [ObservableProperty] private ISearchService _searchService;
        [ObservableProperty] private IPageService _pageService;

        [ObservableProperty] private bool _isUpdateAvailable = false;
        public ILocalizationService L { get; }

        public MainWindowViewModel(
            INavigationService navService,
            ILocalizationService localizationService,
            ISearchService searchService,
            IPageService pageService)
        {
            _navService = navService;
            L = localizationService;
            _searchService = searchService;
            _pageService = pageService;

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
                    await AutoUpdater.CheckForUpdatesAsync();

                    if (AutoUpdater.UpdateAvailable)
                    {
                        await AutoUpdater.DownloadUpdateAsync();
                    }

                    Dispatcher.UIThread.Post(() =>
                    {
                        if (AutoUpdater.UpdateAvailable)
                        {
                            IsUpdateDownloaded = true;
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
            PageService.CurrentPageType = typeof(RecipeExplorerViewModel);
        }

        [RelayCommand]
        private void GoToFavorites()
        {
            NavService.NavigateTo<FavoritesViewModel>();
            PageService.CurrentPageType = typeof(FavoritesViewModel);
        }

        [RelayCommand]
        private void GoToSettings()
        {
            NavService.NavigateTo<SettingsViewModel>();
            PageService.CurrentPageType = typeof(SettingsViewModel);
        }

        [RelayCommand]
        private void GoToAbout()
        {
            NavService.NavigateTo<AboutViewModel>();
            PageService.CurrentPageType = typeof(AboutViewModel);
        }

        [RelayCommand]
        private void AddRecipe()
        {
            NavService.NavigateTo<AddRecipeViewModel>();
            PageService.CurrentPageType = typeof(AddRecipeViewModel);
        }

        [RelayCommand]
        private void UpdateApp()
        {
            AutoUpdater.UpdateAndRestartApp();
        }
    }
}
