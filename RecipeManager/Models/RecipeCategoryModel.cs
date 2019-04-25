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

        public List<RecipeCategory> GetRecipeCategories()
        {
            SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM RecipeCategory", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                List<RecipeCategory> recipeCategories = new List<RecipeCategory>();
                while (sqlDataReader.Read())
                {
                    RecipeCategory recipeCategory = new RecipeCategory
                    {
                        Id = (int)sqlDataReader[0],
                        Name = (string)sqlDataReader[1]
                    };

                    Console.WriteLine(String.Format("{0}, {1}", sqlDataReader[0], sqlDataReader[1]));
                    recipeCategories.Add(recipeCategory);
                }

                return recipeCategories;
            }
            finally
            {
                sqlDataReader.Close();
            }
        }

        public RecipeCategory CreateRecipeCategory(string Name)
        {
            int id = GetNextIdRecipeCategory();
            SqlCommand c = new SqlCommand("INSERT INTO RecipeCategory (Id, Name) VALUES(@ID, @NAME)", sqlConnection);
            c.CommandTimeout = 15;
            c.Parameters.AddWithValue("@ID", id);
            c.Parameters.AddWithValue("@NAME", Name);

            c.ExecuteNonQuery();
            return GetRecipeCategory(id);
        }

        public void DeleteRecipeCategory()
        {
            SqlCommand c = new SqlCommand("DELETE FROM RecipeCategory", sqlConnection);
            c.ExecuteNonQuery();
        }

        public int GetNextIdRecipeCategory()
        {
            /* This query will return null, if the table is empty! */
            SqlCommand c = new SqlCommand("SELECT MAX(Id) FROM RecipeCategory", sqlConnection);
            object obj = c.ExecuteScalar();
            if (obj is System.DBNull)
            {
                return 1;
            }
            else
            {
                return (int)c.ExecuteScalar() + 1;
            }
        }
    }

    public class RecipeCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}