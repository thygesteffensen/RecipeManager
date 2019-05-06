using System.Collections.Generic;
using System.Data.SqlClient;
using RecipeManager.Models;
using RecipeManager.Views;

namespace RecipeManager.Controllers
{
    public class RecipeController
    {
        private readonly CreateRecipe _createRecipe;
        private readonly SqlConnection _sqlConnection;

        private RecipeModel _recipeModel;
        private CommodityModel _commodityModel;

        public RecipeController(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
            this._createRecipe = new CreateRecipe(this);
            _createRecipe.ShowDialog();
        }

        public void CreateRecipe(List<CommodityShadow> commodityList, string recipeName, string recipeDescription,
            RecipeCategory recipeCategory)
        {
            RCModel rcModel = new RCModel(_sqlConnection);
            RecipeCommodityModel recipeCommodityModel = new RecipeCommodityModel(_sqlConnection);
            RecipeModel recipeModel = new RecipeModel(_sqlConnection);
            CommodityModel commodityModel = new CommodityModel(_sqlConnection);


            Recipe temp = _recipeModel.CreateRecipe(recipeName, recipeDescription);
            rcModel.CreateRC(temp, recipeCategory);

            // Now we need to handle the commodities...
            foreach (var commodityShadow in commodityList)
            {
                if (commodityShadow.Commodity != null)
                {
                    recipeCommodityModel.CreateRecipeCommodity(temp, commodityShadow.Commodity, commodityShadow.Value,
                        commodityShadow.Unit.ToString());
                }
                else
                {
                    Commodity commodity = _commodityModel.CreateCommodity(commodityShadow.Name);
                    recipeCommodityModel.CreateRecipeCommodity(temp, commodity, commodityShadow.Value,
                        commodityShadow.Unit.ToString());
                }
            }
        }

        public List<RecipeCategory> GetRecipeCategories()
        {
            RecipeCategoryModel recipeCategory = new RecipeCategoryModel(_sqlConnection);
            return recipeCategory.GetRecipeCategories();
        }

        public List<Commodity> GetCommodities()
        {
            CommodityModel commodity = new CommodityModel(_sqlConnection);
            return commodity.GetCommodities();
        }
    }
}