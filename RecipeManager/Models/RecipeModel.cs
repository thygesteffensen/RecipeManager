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
        SqlConnection sqlConnection;
		public RecipeModel(SqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public Recipe GetRecipe(int ID)
        {
            SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM Recipe WHERE Id={ID}", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                if (sqlDataReader.Read())
                {
                    Recipe recipe = new Recipe
                    {
                        Id = (int)sqlDataReader[0],
                        Name = (string)sqlDataReader[1],
                        Description = (string)sqlDataReader[2]
                    };

                    return recipe;
                }
            }
            finally
            {
                sqlDataReader.Close();
            }
            return null;
        }

        public void CreateRecipe(string Name, string Description)
        {
            SqlCommand c = new SqlCommand("INSERT INTO Recipe (Id, Name, Description) VALUES(@ID, @NAME, @DESCRIPTION)", sqlConnection);
            c.CommandTimeout = 15;
            c.Parameters.AddWithValue("@ID", getNextIDRecipes() + 1);
            c.Parameters.AddWithValue("@NAME", Name);
            c.Parameters.AddWithValue("@DESCRIPTION", Description);

            c.ExecuteNonQuery();
        }

        public void DeleteRecipe()
        {
            SqlCommand c = new SqlCommand("DELETE FROM Recipe", sqlConnection);
            c.ExecuteNonQuery();
        }

        public int getNextIDRecipes()
        {
            SqlCommand c = new SqlCommand("SELECT MAX(Id) FROM Recipe", sqlConnection);
            return (int)c.ExecuteScalar();
        }
    }

    public class Recipe
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
