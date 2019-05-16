using System;
using System.Collections.Generic;
using System.Transactions;
using RecipeManager.Models;
using RecipeManager.Views;

namespace RecipeManager.Viewmodel
{
    public class RecipeVM
    {
        private readonly CreateRecipe _createRecipe;
        private readonly string _dbPath;


        public RecipeVM(string dbPath, string title, Recipe recipe = null)
        {
            this._dbPath = dbPath;
            this._createRecipe = new CreateRecipe(this, recipe);
            _createRecipe.Title = title;
            _createRecipe.ShowDialog();
        }

        public RecipeToCategory GetRecipeCategory(Recipe recipe)
        {
            RCModel rcModel = new RCModel(_dbPath);
            return rcModel.GetCategory(recipe);
        }

        /*
         * This comment belongs to the functions CreateRecipe, UpdateRecipe, RecipeToDb
         * ============================================================================
         * The three functions could have been merged to one and the deleting block
         * could be moved into the else block, when checking for null in recipe variable.
         *
         * I chose to have three functions, to make it more obvious for the maintainer,
         * what the functions did and when they could/should be used.*
         */
        public void CreateRecipe(List<CommodityShadow> commodityList, string recipeName, string recipeDescription,
            RecipeCategory recipeCategory)
        {
            RecipeToDb(commodityList, recipeName, recipeDescription, recipeCategory);
        }

        public void UpdateRecipe(List<CommodityShadow> commodityList, string recipeName, string recipeDescription,
            RecipeCategory recipeCategory, Recipe recipe)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                RCModel rcModel = new RCModel(_dbPath);
                RecipeCommodityModel recipeCommodityModel = new RecipeCommodityModel(_dbPath);
                RecipeModel recipeModel = new RecipeModel(_dbPath);

                // We do not want to delete the recipe, since this will change its place
                // on the list, but we delete all the connections to other tuples!
                rcModel.DeleteRC(recipe);

                recipeCommodityModel.DeleteRecipeCommodities(recipe);

                recipeModel.EditRecipe(recipe, recipeName, recipeDescription);

                RecipeToDb(commodityList, recipeName, recipeDescription, recipeCategory, recipe);

                scope.Complete();
            }
        }

        private void RecipeToDb(List<CommodityShadow> commodityList, string recipeName, string recipeDescription,
            RecipeCategory recipeCategory, Recipe recipe = null)
        {
            RCModel rcModel = new RCModel(_dbPath);
            RecipeCommodityModel recipeCommodityModel = new RecipeCommodityModel(_dbPath);
            RecipeModel recipeModel = new RecipeModel(_dbPath);
            CommodityModel commodityModel = new CommodityModel(_dbPath);

            if (recipe == null)
            {
                recipe = recipeModel.CreateRecipe(recipeName, recipeDescription);
            }

            rcModel.CreateRC(recipe, recipeCategory);

            foreach (var commodityShadow in commodityList)
            {
                if (commodityShadow.Commodity != null)
                {
                    recipeCommodityModel.CreateRecipeCommodity(recipe, commodityShadow.Commodity, commodityShadow.Value,
                        commodityShadow.Unit.ToString());
                }
                else
                {
                    Commodity commodity = commodityModel.CreateCommodity(commodityShadow.Name);
                    recipeCommodityModel.CreateRecipeCommodity(recipe, commodity, commodityShadow.Value,
                        commodityShadow.Unit.ToString());
                }
            }
        }

        public List<CommodityShadow> GetCommoditiesFromRecipe(Recipe recipe)
        {
            RecipeCommodityModel recipeCommodityModel = new RecipeCommodityModel(_dbPath);
            List<RecipeCommodity> list = recipeCommodityModel.GetRecipeCommodity(recipe);

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