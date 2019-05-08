using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using RecipeManager.Models;
using RecipeManager.Views;

namespace RecipeManager.Controllers
{
    public class RecipeController
    {
        private readonly CreateRecipe _createRecipe;
        private readonly string _dbPath;


        public RecipeController(string dbPath, Recipe recipe=null)
        {
            this._dbPath = dbPath;
            this._createRecipe = new CreateRecipe(this, recipe);
            _createRecipe.ShowDialog();

            // If there is given is recipe, this hould be loaded
//            if (recipe != null)
//            {
//                RCModel rcModel = new RCModel(dbPath);
//                RecipeToCategory recipeToCategory = rcModel.GetCategory(recipe);
//            }
        }

        public RecipeToCategory GetRecipeCategory(Recipe recipe)
        {
            RCModel rcModel = new RCModel(_dbPath);
            return rcModel.GetCategory(recipe);
        }

        public void CreateRecipe(List<CommodityShadow> commodityList, string recipeName, string recipeDescription,
            RecipeCategory recipeCategory)
        {
            RCModel rcModel = new RCModel(_dbPath);
            RecipeCommodityModel recipeCommodityModel = new RecipeCommodityModel(_dbPath);
            RecipeModel recipeModel = new RecipeModel(_dbPath);
            CommodityModel commodityModel = new CommodityModel(_dbPath);


            Recipe temp = recipeModel.CreateRecipe(recipeName, recipeDescription);
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
                    Commodity commodity = commodityModel.CreateCommodity(commodityShadow.Name);
                    recipeCommodityModel.CreateRecipeCommodity(temp, commodity, commodityShadow.Value,
                        commodityShadow.Unit.ToString());
                }
            }
        }

        public List<CommodityShadow> GetCommoditiesFromRecipe(Recipe recipe)
        {
            RecipeCommodityModel recipeCommodityModel = new RecipeCommodityModel(_dbPath);
            List<RecipeCommodity> list =  recipeCommodityModel.GetRecipeCommodity(recipe);

            List<CommodityShadow> list_1 = new List<CommodityShadow>();
            foreach (RecipeCommodity recipeCommodity in list)
            {
                Units unit;
                try
                {
                    unit = (Units) Enum.Parse(typeof(Units), recipeCommodity.Unit);
                }
                catch (ArgumentException)
                {
                    unit = Units.stk; // Just something
                }

                list_1.Add(new CommodityShadow
                {
                    Id = recipeCommodity.Recipe.Id,
                    Commodity = recipeCommodity.Commodity,
                    Name = recipeCommodity.Commodity.Name,
                    Unit = unit,
                    Value = recipeCommodity.Value
                });
            }
            return list_1;
        }

        public List<RecipeCategory> GetRecipeCategories()
        {
            RecipeCategoryModel recipeCategory = new RecipeCategoryModel(_dbPath);
            return recipeCategory.GetRecipeCategories();
        }

        public List<Commodity> GetCommodities()
        {
            CommodityModel commodity = new CommodityModel(_dbPath);
            return commodity.GetCommodities();
        }
    }
}