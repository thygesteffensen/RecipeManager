using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    class RecipeCategoryModel
    {
        SqlConnection sqlConnection;
        public RecipeCategoryModel(SqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public RecipeCategory GetRecipeCategory(int ID)
        {
            SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM RecipeCategory WHERE Id={ID}", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                if (sqlDataReader.Read())
                {
                    RecipeCategory recipeCategory = new RecipeCategory
                    {
                        Id = (int)sqlDataReader[0],
                        Name = (string)sqlDataReader[1]
                    };

                    Console.WriteLine(String.Format("{0}, {1}", sqlDataReader[0], sqlDataReader[1]));
                    return recipeCategory;
                }
            }
            finally
            {
                sqlDataReader.Close();
            }
            return null;
        }

        public RecipeCategory CreateRecipeCategory(string Name)
        {
            SqlCommand c = new SqlCommand("INSERT INTO RecipeCategory (Id, Name) VALUES(@ID, @NAME)", sqlConnection);
            c.CommandTimeout = 15;
            c.Parameters.AddWithValue("@ID", getNextIDRecipeCategory() + 1);
            c.Parameters.AddWithValue("@NAME", Name);

            c.ExecuteNonQuery();
            return null;
        }

        public void DeleteRecipeCategory()
        {
            SqlCommand c = new SqlCommand("DELETE FROM RecipeCategory", sqlConnection);
            c.ExecuteNonQuery();
        }

        public int getNextIDRecipeCategory()
        {
            SqlCommand c = new SqlCommand("SELECT MAX(Id) FROM RecipeCategory", sqlConnection);
            return (int)c.ExecuteScalar();
        }
    }

    public class RecipeCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}