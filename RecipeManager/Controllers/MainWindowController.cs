using System.Collections.Generic;
using System.Data.SqlClient;
using RecipeManager.Models;


namespace RecipeManager.Controllers
{
    class MainWindowController
    {
        SqlConnection conn;   // A connection which is used by all interactions with the DB.

        public MainWindowController()
        {
            // TODO: Need to find a way to keep this part dynamic!
            //string path = Path.Combine(Application.StartupPath, "RecipeMangerDatabase.mdf");
            //string startupPath = Environment.CurrentDirectory;
            //string path = startupPath + "\\RecipeMangerDatabase.mdf";
            string path = "C:\\Users\\Thyge Steffensen\\Documents\\RecipeManager\\RecipeManager\\RecipeManagerDatabase.mdf";

            //SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + path + ";");
            conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"" + path + "\";Integrated Security=True");
            conn.Open();
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
