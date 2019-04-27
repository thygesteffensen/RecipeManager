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
        private RecipeCategoryModel recipeCategoryModel;

        public RecipeCategoryController(SqlConnection sqlConnection)
        {
            recipeCategoryModel = new RecipeCategoryModel(sqlConnection);
        }

        public List<RecipeCategory> GetRecipeCategories()
        {
            return recipeCategoryModel.GetRecipeCategories();
        }

        public void CreateRecipeCategory(string recipeCategoryName)
        {
            recipeCategoryModel.CreateRecipeCategory(recipeCategoryName);
        }
    }
}
