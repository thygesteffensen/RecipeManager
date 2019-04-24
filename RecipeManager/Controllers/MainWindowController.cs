using System.Collections.Generic;
using System.Data.SqlClient;
using RecipeManager.Models;


namespace RecipeManager.Controllers
{
    class MainWindowController
    {
        SqlConnection sqlConnection;   // A connection which is used by all interactions with the DB.

        public MainWindowController()
        {
            // TODO: Need to find a way to keep this part dynamic!
            //string path = Path.Combine(Application.StartupPath, "RecipeMangerDatabase.mdf");
            //string startupPath = Environment.CurrentDirectory;
            //string path = startupPath + "\\RecipeMangerDatabase.mdf";
            string path = "C:\\Users\\Thyge Steffensen\\Documents\\RecipeManager\\RecipeManager\\RecipeManagerDatabase.mdf";

            //SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + path + ";");
            sqlConnection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"" + path + "\";Integrated Security=True");
            sqlConnection.Open();
        }

        private int i = 0;
        public List<Category> GetCategories()
        {
            // THis hould containt a DB model call
            List<Category> list = new List<Category>();

            for (int j = 1; j < 25; j++, i++)
            {
                list.Add(new Category(){Title = $"Katogri {i}", Id = i});
            }

            return list;
        }

        public List<Recipe> GetRecipes(Category category)
        {
            List<Recipe> list = new List<Recipe>();

            for (int j = 1; j < 25; j++, i++)
            {
                list.Add(new Recipe() { Title = $"Katogri {i}", Id = i, Description = " Lorm Bacon Imspum Sadel med mere...."});
            }

            return list;
        }

        public void PopulateDBDummyData()
        {
            // All Model are initialized
            CCCModel cccModel = new CCCModel(sqlConnection);
            CommodityCategoryModel commodityCategoryModel = new CommodityCategoryModel(sqlConnection);
            CommodiyModel commodiytModel = new CommodiyModel(sqlConnection);
            RCModel rcModel = new RCModel(sqlConnection);
            RecipeCategoryModel recipeCategoryModel = new RecipeCategoryModel(sqlConnection);
            RecipeCommodityModel recipeCommodityModel = new RecipeCommodityModel(sqlConnection);
            RecipeModel recipeModel = new RecipeModel(sqlConnection);
            // All data is wiped
            rcModel.DeleteRC();
            cccModel.DeleteCCC();
            recipeCommodityModel.DeleteRecipeCommodity();
            recipeCategoryModel.DeleteRecipeCategory();
            recipeModel.DeleteRecipe();
            commodiytModel.DeleteCommodity();
            commodityCategoryModel.DeleteCommodityCategory();
            // Add some cool recipes yes.
            // Now we will add Lasagna!
            recipeCategoryModel.CreateRecipeCategory("Aftensmad");
            recipeModel.CreateRecipe("Lasagne", "Kødsovs: Svits kødet i olie i en bred gryde, til det har skiftet farve.\nPil og hak løgene. Skræl gulerod og selleri og hak det fint. Svits løg, hvidløg og grøntsager et par minutter. Tilsæt lauerbærblade, oregano, timian, salt og peber.\nOstesauce: Semlt smærret i en gryde. Drys melet i og pisk blandingen sammen. Pisk mælken i lidt ad gangen og kog saucen igennem nogle minutter. Rør osten i.\nSmag til med revet muskat, salt og peber.\nSmør et ovnfast fad eller en lille bradepande.\nSaml lasagnen: Læg lasagneplader, kødsauce og ostesauce = spring evt. ostesaucen over i midten, det er vigtigt, at der er nok til at dzkke overfladen. Drys parmesan over.\nLad lasagnen trække ca. 20 in. (eller til pladerne er bløde), inden den kommer i ovnen. Bag lasagnen i ovnen ved 180 grader i ca. 50 min. Lad den hvile 5 min. inden udskæring.\n\nTIP: Hvis man bruger friske lasagneplader, skal lasagnen ikke trække i 20 m in.\nTilsæt evt. flere grøntsager til kødsaucen, fx 1 squasch, 1 peberfrugt, el. 1 pastinak, skåret i tern.");
            rcModel.CreateRC()


            commodiytModel.GetCommodity(1);
            commodiytModel.CreateCommodity("Skinkesalat");
        }
    }

    /// <summary>
    /// This class stores the loaded categories.
    /// </summary>
    public class Category
    {
        public string Title { get; set; }
        public int Id { get; set; }
    }
}
