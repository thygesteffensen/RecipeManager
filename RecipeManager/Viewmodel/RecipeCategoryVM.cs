using System.Collections.Generic;
using RecipeManager.Models;

namespace RecipeManager.Viewmodel
{
    internal class RecipeCategoryVM
    {
        private readonly RecipeCategoryModel _recipeCategoryModel;

        public RecipeCategoryVM(string dbPath)
        {
            _recipeCategoryModel = new RecipeCategoryModel(dbPath);
        }

        public List<RecipeCategory> GetRecipeCategories()
        {
            return _recipeCategoryModel.GetRecipeCategories();
        }

        public RecipeCategory CreateRecipeCategory(string recipeCategoryName)
        {
            return _recipeCategoryModel.CreateRecipeCategory(recipeCategoryName);
        }
    }
}