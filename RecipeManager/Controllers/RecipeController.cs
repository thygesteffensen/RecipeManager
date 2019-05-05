using System.Collections.Generic;
using System.Data.SqlClient;
using RecipeManager.Models;
using RecipeManager.Views;

namespace RecipeManager.Controllers
{
    class RecipeController
    {
        private RecipeModel _recipeModel;
        private RCModel _rcModel;
        private RecipeCommodityModel _recipeCommodityModel;
        private CommodityModel _commodityModel;

        public RecipeController(SqlConnection sqlConnection)
        {
            _recipeModel = new RecipeModel(sqlConnection);
            _rcModel = new RCModel(sqlConnection);
            _recipeCommodityModel = new RecipeCommodityModel(sqlConnection);
            _commodityModel = new CommodityModel(sqlConnection);
        }

        public void CreateRecipe(List<CommodityShadow> commodityList, string recipeName, string recipeDescription,
            RecipeCategory recipeCategory)
        {
            Recipe temp = _recipeModel.CreateRecipe(recipeName, recipeDescription);
            _rcModel.CreateRC(temp, recipeCategory);

            // Now we need to handle the commodities...
            foreach (var commodityShadow in commodityList)
            {
                if (commodityShadow.Commodity != null)
                {
                    _recipeCommodityModel.CreateRecipeCommodity(temp, commodityShadow.Commodity, commodityShadow.Value,
                        commodityShadow.Unit.ToString());
                }
                else
                {
                    Commodity commodity = _commodityModel.CreateCommodity(commodityShadow.Name);
                    _recipeCommodityModel.CreateRecipeCommodity(temp, commodity, commodityShadow.Value,
                        commodityShadow.Unit.ToString());
                }
            }
        }
    }
}