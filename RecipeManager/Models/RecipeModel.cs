using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeManager;
using System.Data.SqlClient;

namespace RecipeManager.Models
{
	class RecipeModel
	{
		private List<Recipe> _listRecipes = new List<Recipe>();

		public List<Recipe> GetRecipes()
		{
			return _listRecipes;
		}

		public void SaveRecipe(Recipe recipe)
		{
			_listRecipes.Add(recipe);
		}

		public void AddToDB()
		{
			//string path = Path.Combine(Application.StartupPath, "LocalDataBase.mdf");
			string path = "C:\\Users\\Thyge Steffensen\\Documents\\RecipeManager\\RecipeManager\\LocalDataBase.mdf";

			SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + path + ";");
			conn.Open();

			SqlCommand command = new SqlCommand("SELECT * FROM Recipe", conn);

			SqlDataReader reader = command.ExecuteReader();

			Console.Write("Database result will be printed here:");
			while (reader.Read())
			{
				string text = reader.GetString(0);
				Console.Write(text);
			}
			/*
			SqlCommand c = new SqlCommand("INSERT INTO DATA (id, name) VALUES(i, v)", conn);
			c.Parameters.AddWithValue("@i", 1);
			c.Parameters.AddWithValue("@v", "Jack");

			c.ExecuteNonQuery();
			*/
			conn.Dispose();
		}

	}

    public class Recipe
    {
        public List<Comodity> Comodities { get; set; }
        public string Title { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class Comodity
    {
        public string ComodityName { get; set; }
        public int Id { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }

        public override string ToString()
        {
            return $"{Value} {Unit} {ComodityName} ({Id})";
        }
    }
}
