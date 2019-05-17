using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Transactions;
using RecipeManager.Models;
using RecipeManager.Views;

namespace RecipeManager.Viewmodel
{
    internal class MainWindowVM
    {
        private readonly string _dbPath; // A connection which is used by all interactions with the DB.

        public MainWindowVM()
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
            var createRecipeCategory = new CreateRecipeCategory(_dbPath);
            createRecipeCategory.ShowDialog();
        }

        public bool DeleteRecipe(Recipe recipe)
        {
            try
            {
                var recipeModel = new RecipeModel(_dbPath);
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
                var recipeVm = new RecipeVM(_dbPath, $"Ændre {recipe.Name}", recipe);
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
            var recipeCategoryModel = new RecipeCategoryModel(_dbPath);


            return recipeCategoryModel.GetRecipeCategories();
        }

        public List<Recipe> GetRecipes(RecipeCategory recipeCategory)
        {
            var recipeModel = new RecipeModel(_dbPath);

            return recipeModel.GetRecipes(recipeCategory);
        }

        public void OpenScrapeLink()
        {
            var scrapeVm = new ScrapeVM(_dbPath);
        }

        public void OpenCreateRecipeWindow()
        {
            var recipeVm = new RecipeVM(_dbPath, "Opret opskrift");
        }

        public void DeleteAllContent()
        {
            // All Model are initialized
            var commodiytModel = new CommodityModel(_dbPath);
            var rcModel = new RCModel(_dbPath);
            var recipeCategoryModel = new RecipeCategoryModel(_dbPath);
            var recipeCommodityModel = new RecipeCommodityModel(_dbPath);
            var recipeModel = new RecipeModel(_dbPath);
            // All data is wiped
            rcModel.DeleteAllRC();
            recipeCommodityModel.DeleteAllRecipeCommodities();
            recipeCategoryModel.DeleteRecipeCategory();
            recipeModel.DeleteRecipes();
            commodiytModel.DeleteCommodity();
        }

        public List<RecipeCommodity> GetCommodities(Recipe recipe)
        {
            var recipeCommodityModel = new RecipeCommodityModel(_dbPath);
            return recipeCommodityModel.GetRecipeCommodity(recipe);
        }

        public void PopulateDBDummyData()
        {
            DeleteAllContent();
            // All Model are initialized
            var commodiytModel = new CommodityModel(_dbPath);
            var rcModel = new RCModel(_dbPath);
            var recipeCategoryModel = new RecipeCategoryModel(_dbPath);
            var recipeCommodityModel = new RecipeCommodityModel(_dbPath);
            var recipeModel = new RecipeModel(_dbPath);
            // Add some cool recipes yes.

            // Creating Categories
            var recipeCategoryAftensmad = recipeCategoryModel.CreateRecipeCategory("Aftensmad");
            var recipeCategoryDessert = recipeCategoryModel.CreateRecipeCategory("Dessert");

            // Creating some recipies
            var recipeLasagna = recipeModel.CreateRecipe("Lasagne",
                "Kødsovs: Svits kødet i olie i en bred gryde, til det har skiftet farve.\nPil og hak løgene. Skræl gulerod og selleri og hak det fint. Svits løg, hvidløg og grøntsager et par minutter. Tilsæt lauerbærblade, oregano, timian, salt og peber.\nOstesauce: Semlt smærret i en gryde. Drys melet i og pisk blandingen sammen. Pisk mælken i lidt ad gangen og kog saucen igennem nogle minutter. Rør osten i.\nSmag til med revet muskat, salt og peber.\nSmør et ovnfast fad eller en lille bradepande.\nSaml lasagnen: Læg lasagneplader, kødsauce og ostesauce = spring evt. ostesaucen over i midten, det er vigtigt, at der er nok til at dzkke overfladen. Drys parmesan over.\nLad lasagnen trække ca. 20 in. (eller til pladerne er bløde), inden den kommer i ovnen. Bag lasagnen i ovnen ved 180 grader i ca. 50 min. Lad den hvile 5 min. inden udskæring.\n\nTIP: Hvis man bruger friske lasagneplader, skal lasagnen ikke trække i 20 m in.\nTilsæt evt. flere grøntsager til kødsaucen, fx 1 squasch, 1 peberfrugt, el. 1 pastinak, skåret i tern.");

            var recipeBananaSplit = recipeModel.CreateRecipe("Banana split",
                "Lav chokoladesirup: \n" +
                "Bring alle ingredienserne i kog under omrøring. Køl det så af. Chokoladesiruppen kan holde sig længe i køleskabet.\n\n" +
                "Lav bananasplit: \n" +
                "Flæk bananen og læg hver halvdel på langs i skålen. En klassisk banana split skal helst serveres i en aflang glasskål af nogenlunde samme længde som bananen. \n" +
                "Læg tre kugler is i midten, en kugle vanille-, en kugle chokolade- og en kugle jordbæris. Pynt vanilleisen med chokoladesirup, jordbærisen med hakket ananas og chokoladeisen med jordbærskiver. Pynt yderligere med flødeskum, hakkede hasselnødder, mere chokoladesirup og 2-3 røde cocktailbær. En banana split skal være stor og vulgær at se på - så hold endelige ikke igen!");

            // Now we need to bind them together! We do need these objects :D
            rcModel.CreateRC(recipeLasagna, recipeCategoryAftensmad);
            rcModel.CreateRC(recipeBananaSplit, recipeCategoryDessert);

            var salt = commodiytModel.CreateCommodity("Salt");

            // Adding commodities
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Lasagneplader"),
                250, Units.g.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Hakket oksekød"),
                500, Units.g.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Olie"), 1,
                Units.spsk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Løg"), 2,
                Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Gulerod"), 1,
                Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Selleri"), 100,
                Units.g.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Laurbærblade"), 1,
                Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Oregano"), 2,
                Units.tsk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Timian"), 1,
                Units.tsk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, salt, 0.75, Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Peber"), 1,
                Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Hvidløg"), 3,
                Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Hakkede tomater"),
                1, Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Tomatpure"), 3,
                Units.spsk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Oksebuillon"), 3,
                Units.dl.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Smør"), 25,
                Units.g.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Hvedemel"), 3,
                Units.spsk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Mælk"), 6,
                Units.dl.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Muskatnød"), 1,
                Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeLasagna, commodiytModel.CreateCommodity("Parmasan"), 50,
                Units.g.ToString());


            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit, commodiytModel.CreateCommodity("Banan"), 1,
                Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit, commodiytModel.CreateCommodity("Vanile is"),
                1, Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit,
                commodiytModel.CreateCommodity("Chokolade is"), 1, Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit, commodiytModel.CreateCommodity("Jordbær is"),
                1, Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit, commodiytModel.CreateCommodity("Ananas"), 1,
                Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit, commodiytModel.CreateCommodity("Jordbær"), 1,
                Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit,
                commodiytModel.CreateCommodity("Chokoladesirup"), 1, Units.spsk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit,
                commodiytModel.CreateCommodity("Hassel nødder"), 1, Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit, commodiytModel.CreateCommodity("cocktailbær"),
                3, Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit, commodiytModel.CreateCommodity("Vand"), 2,
                Units.dl.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit, commodiytModel.CreateCommodity("Sukker"), 150,
                Units.g.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit, commodiytModel.CreateCommodity("Kakao"), 60,
                Units.g.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit,
                commodiytModel.CreateCommodity("Vanileessens"), 1, Units.stk.ToString());
            recipeCommodityModel.CreateRecipeCommodity(recipeBananaSplit, salt, 5, Units.g.ToString());
        }
    }
}