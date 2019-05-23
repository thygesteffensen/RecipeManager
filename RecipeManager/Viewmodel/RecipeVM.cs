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
        private Func<Recipe, int> callback;


        public RecipeVM(string dbPath, string title, Func<Recipe, int> callback, Recipe recipe = null)
        {
            _dbPath = dbPath;
            _createRecipe = new CreateRecipe(this, recipe);
            _createRecipe.Title = title;
            _createRecipe.ShowDialog();
            this.callback = callback;
        }

        public RecipeToCategory GetRecipeCategory(Recipe recipe)
        {
            var rcModel = new RCModel(_dbPath);
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
            using (var scope = new TransactionScope())
            {
                var rcModel = new RCModel(_dbPath);
                var recipeCommodityModel = new RecipeCommodityModel(_dbPath);
                var recipeModel = new RecipeModel(_dbPath);

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
            var rcModel = new RCModel(_dbPath);
            var recipeCommodityModel = new RecipeCommodityModel(_dbPath);
            var recipeModel = new RecipeModel(_dbPath);
            var commodityModel = new CommodityModel(_dbPath);

            if (recipe == null)
            {
                recipe = recipeModel.CreateRecipe(recipeName, recipeDescription);
                callback(recipe);
            }
            

            rcModel.CreateRC(recipe, recipeCategory);

            foreach (var commodityShadow in commodityList)
                if (commodityShadow.Commodity != null)
                {
                    recipeCommodityModel.CreateRecipeCommodity(recipe, commodityShadow.Commodity, commodityShadow.Value,
                        commodityShadow.Unit.ToString());
                }
                else
                {
                    var commodity = commodityModel.CreateCommodity(commodityShadow.Name);
                    recipeCommodityModel.CreateRecipeCommodity(recipe, commodity, commodityShadow.Value,
                        commodityShadow.Unit.ToString());
                }
        }

        public List<CommodityShadow> GetCommoditiesFromRecipe(Recipe recipe)
        {
            var recipeCommodityModel = new RecipeCommodityModel(_dbPath);
            var list = recipeCommodityModel.GetRecipeCommodity(recipe);

            var list_1 = new List<CommodityShadow>();
            foreach (var recipeCommodity in list)
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
            var recipeCategory = new RecipeCategoryModel(_dbPath);
            return recipeCategory.GetRecipeCategories();
        }

        public List<Commodity> GetCommodities()
        {
            var commodity = new CommodityModel(_dbPath);
            return commodity.GetCommodities();
        }
    }
}