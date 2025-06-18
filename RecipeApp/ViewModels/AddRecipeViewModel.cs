using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeApp.Models;
using RecipeApp.Services;
using RecipeApp.Services.Localization;
using RecipeApp.Services.Navigation;

namespace RecipeApp.ViewModels
{
    public partial class AddRecipeViewModel : ViewModelBase
    {
        [ObservableProperty] private INavigationService _navService;
        [ObservableProperty] private Recipe _recipeDraft;
        [ObservableProperty] private Ingredient _ingredientDraft;

        [ObservableProperty] private bool _isPopupOpen;

        private readonly IRecipeDataService _recipeDataService;
        private readonly Recipe? loadedRecipe;

        public ILocalizationService L { get; }

        public AddRecipeViewModel(INavigationService navService, IRecipeDataService recipeDataService, ILocalizationService localizationService, Recipe? recipe = null)
        {
            _navService = navService;
            _recipeDataService = recipeDataService;
            L = localizationService;

            loadedRecipe = recipe;

            if (recipe == null)
            {
                RecipeDraft = new Recipe();
                RecipeDraft.ImagePath = "avares://RecipeApp/Assets/Images/food_placeholder.png";
            }
            else
            {
                RecipeDraft = recipe;
            }

            IngredientDraft = new Ingredient();
        }

        [RelayCommand]
        private void UploadImage()
        {
            if (IsPopupOpen)
            {
                //Do Stuff
                IsPopupOpen = false;
            }
            else
            {
                IsPopupOpen = true;
            }
        }

        [RelayCommand]
        private void CancelUpload()
        {
            IsPopupOpen = false;
        }

        [RelayCommand]
        private void AddIngredient()
        {
            RecipeDraft.Ingredients.Add(IngredientDraft);
            IngredientDraft = new();
        }

        [RelayCommand]
        private void RemoveIngredient(Ingredient ingredient)
        {
            RecipeDraft.Ingredients.Remove(ingredient);
        }

        [RelayCommand]
        private void AddRecipe()
        {
            if (loadedRecipe != null && RecipeDraft.Id == loadedRecipe.Id)
            {
                _recipeDataService.UpdateRecipe(RecipeDraft);
            }
            else
            {
                _recipeDataService.AddRecipe(RecipeDraft);
            }

            NavService.NavigateTo<RecipeExplorerViewModel>();
        }

        [RelayCommand]
        private void CancelRecipe()
        {
            NavService.NavigateTo<RecipeExplorerViewModel>();
        }

    }
}
