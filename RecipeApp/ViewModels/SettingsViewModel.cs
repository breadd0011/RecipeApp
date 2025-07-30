using RecipeApp.Services.Localization;

namespace RecipeApp.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public ILocalizationService L { get; }
        public SettingsViewModel(ILocalizationService localizationService)
        {
            L = localizationService;
        }
    }
}
