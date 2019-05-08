using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeManager.Models;

namespace RecipeManager.Controllers
{
    class RecipeCategoryController
    {
        private readonly RecipeCategoryModel _recipeCategoryModel;

        public RecipeCategoryController(string dbPath)
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
