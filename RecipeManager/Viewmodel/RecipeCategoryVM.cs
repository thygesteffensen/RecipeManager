using System.Collections.Generic;
using RecipeManager.Models;

namespace RecipeManager.Viewmodel
{
    class RecipeCategoryVM
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

        public void CreateRecipeCategory(string recipeCategoryName)
        {
            _recipeCategoryModel.CreateRecipeCategory(recipeCategoryName);
        }
    }
}
