using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Transactions;
using RecipeManager.Models;
using RecipeManager.Views;


namespace RecipeManager.Controllers
{
    class MainWindowController
    {
        private readonly string _dbPath;// A connection which is used by all interactions with the DB.

        public MainWindowController()
        {
            // Getting the string from the config
            var path = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ToString();
            // Determine the base directory
            var currentDomain = AppDomain.CurrentDomain;
            var basePath = currentDomain.BaseDirectory;
            currentDomain.SetData("DataDirectory", basePath);
            // Combining base dir with path
            _dbPath = path.Replace("|DataDirectory|", basePath);
            // Creating sql connection
        }

        public void OpenCreateRecipeCategoryWindow()
        {
            CreateRecipeCategory createRecipeCategory = new CreateRecipeCategory(_dbPath);
            createRecipeCategory.ShowDialog();
        }

        public bool DeleteRecipe(Recipe recipe)
        {
            try
            {
                RecipeModel recipeModel = new RecipeModel(_dbPath);
                recipeModel.DeleteRecipe(recipe);
                return true;
            }
            catch (SqlException)
            {
                Console.WriteLine("Couldn't delete recipe...");
            }
            catch (TransactionException)
            {
                Console.WriteLine("Couldn't delete recipe...");
            }

            return false;
        }

        public bool EditRecipe(Recipe recipe)
        {
            try
            {
                RecipeController recipeController = new RecipeController(_dbPath, $"Ændre {recipe.Name}", recipe);
                return true;
            }
            catch (SqlException)
            {
                Console.WriteLine("Couldn't alter recipe...");
            }
            catch (TransactionException)
            {
                Console.WriteLine("Couldn't alter recipe...");
            }

            return false;
        }

        public List<RecipeCategory> GetCategories()
        {
            RecipeCategoryModel recipeCategoryModel = new RecipeCategoryModel(_dbPath);


            return recipeCategoryModel.GetRecipeCategories();
        }

        public List<Recipe> GetRecipes(RecipeCategory recipeCategory)
        {
            RecipeModel recipeModel = new RecipeModel(_dbPath);

            return recipeModel.GetRecipes(recipeCategory);
        }

        public void OpenScrapeLink()
        {
            ScrapeController scrapeController = new ScrapeController(_dbPath);
        }

        public void OpenCreateRecipeWindow()
        {
            RecipeController recipeController = new RecipeController(_dbPath, "Opret opskrift");
        }

        public void DeleteAllContent()
        {
            // All Model are initialized
            CCCModel cccModel = new CCCModel(_dbPath);
            CommodityCategoryModel commodityCategoryModel = new CommodityCategoryModel(_dbPath);
            CommodityModel commodiytModel = new CommodityModel(_dbPath);
            RCModel rcModel = new RCModel(_dbPath);
            RecipeCategoryModel recipeCategoryModel = new RecipeCategoryModel(_dbPath);
            RecipeCommodityModel recipeCommodityModel = new RecipeCommodityModel(_dbPath);
            RecipeModel recipeModel = new RecipeModel(_dbPath);
            // All data is wiped
            rcModel.DeleteAllRC();
            cccModel.DeleteCCC();
            recipeCommodityModel.DeleteAllRecipeCommodities();
            recipeCategoryModel.DeleteRecipeCategory();
            recipeModel.DeleteRecipes();
            commodiytModel.DeleteCommodity();
            commodityCategoryModel.DeleteCommodityCategory();
        }

        public List<RecipeCommodity> GetCommodities(Recipe recipe)
        {
            RecipeCommodityModel recipeCommodityModel = new RecipeCommodityModel(_dbPath);
            return recipeCommodityModel.GetRecipeCommodity(recipe);
        }

        public void PopulateDBDummyData()
        {
            DeleteAllContent();
            // All Model are initialized
            CCCModel cccModel = new CCCModel(_dbPath);
            CommodityCategoryModel commodityCategoryModel = new CommodityCategoryModel(_dbPath);
            CommodityModel commodiytModel = new CommodityModel(_dbPath);
            RCModel rcModel = new RCModel(_dbPath);
            RecipeCategoryModel recipeCategoryModel = new RecipeCategoryModel(_dbPath);
            RecipeCommodityModel recipeCommodityModel = new RecipeCommodityModel(_dbPath);
            RecipeModel recipeModel = new RecipeModel(_dbPath);
            // Add some cool recipes yes.

            // Creating Categories
            RecipeCategory recipeCategoryAftensmad = recipeCategoryModel.CreateRecipeCategory("Aftensmad");
            RecipeCategory recipeCategoryDessert = recipeCategoryModel.CreateRecipeCategory("Dessert");

            // Creating some recipies
            Recipe recipeLasagna = recipeModel.CreateRecipe("Lasagne",
                "Kødsovs: Svits kødet i olie i en bred gryde, til det har skiftet farve.\nPil og hak løgene. Skræl gulerod og selleri og hak det fint. Svits løg, hvidløg og grøntsager et par minutter. Tilsæt lauerbærblade, oregano, timian, salt og peber.\nOstesauce: Semlt smærret i en gryde. Drys melet i og pisk blandingen sammen. Pisk mælken i lidt ad gangen og kog saucen igennem nogle minutter. Rør osten i.\nSmag til med revet muskat, salt og peber.\nSmør et ovnfast fad eller en lille bradepande.\nSaml lasagnen: Læg lasagneplader, kødsauce og ostesauce = spring evt. ostesaucen over i midten, det er vigtigt, at der er nok til at dzkke overfladen. Drys parmesan over.\nLad lasagnen trække ca. 20 in. (eller til pladerne er bløde), inden den kommer i ovnen. Bag lasagnen i ovnen ved 180 grader i ca. 50 min. Lad den hvile 5 min. inden udskæring.\n\nTIP: Hvis man bruger friske lasagneplader, skal lasagnen ikke trække i 20 m in.\nTilsæt evt. flere grøntsager til kødsaucen, fx 1 squasch, 1 peberfrugt, el. 1 pastinak, skåret i tern.");

            Recipe recipeSovs = recipeModel.CreateRecipe("Kødsovs", "Kom an");

            Recipe recipeBananaSplit = recipeModel.CreateRecipe("Banana split", "Pisk flødeskom. \nHent Whikeyen.\nTag isen frem.\nPil en banan.\n Anret og server");

            // Now we need to bind them together! We do need these objects :D
            rcModel.CreateRC(recipeLasagna, recipeCategoryAftensmad);
            rcModel.CreateRC(recipeSovs, recipeCategoryAftensmad);
            rcModel.CreateRC(recipeBananaSplit, recipeCategoryDessert);

            // Now we add all the commodities to the db....
            Commodity onion = commodiytModel.CreateCommodity("Løg");
            Commodity meat = commodiytModel.CreateCommodity("Oksekød");

            // Now we will bind them to the recipe
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, onion, 2, "stk");
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, meat, 500, "g");


            commodiytModel.GetCommodity(1);
            commodiytModel.CreateCommodity("Skinkesalat");
        }
    }
}
