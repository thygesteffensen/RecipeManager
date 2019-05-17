using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RecipeManager.Models
{
    internal class RecipeCategoryModel
    {
        private readonly string _dbPath;

        public RecipeCategoryModel(string dbPath)
        {
            _dbPath = dbPath;
        }

        public RecipeCategory GetRecipeCategory(int ID)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand($"SELECT * FROM RecipeCategory WHERE Id={ID}", sqlConnection);
                var sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    if (sqlDataReader.Read())
                    {
                        var recipeCategory = new RecipeCategory
                        {
                            Id = (int) sqlDataReader[0],
                            Name = (string) sqlDataReader[1]
                        };

                        return recipeCategory;
                    }
                }
                finally
                {
                    sqlDataReader.Close();
                }

                return null;
            }
        }

        public List<RecipeCategory> GetRecipeCategories()
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand("SELECT * FROM RecipeCategory", sqlConnection);
                var sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    var recipeCategories = new List<RecipeCategory>();
                    while (sqlDataReader.Read())
                    {
                        var recipeCategory = new RecipeCategory
                        {
                            Id = (int) sqlDataReader[0],
                            Name = (string) sqlDataReader[1]
                        };

                        recipeCategories.Add(recipeCategory);
                    }

                    return recipeCategories;
                }
                finally
                {
                    sqlDataReader.Close();
                }
            }
        }

        public RecipeCategory CreateRecipeCategory(string Name)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var id = GetNextIdRecipeCategory();
                var c = new SqlCommand("INSERT INTO RecipeCategory (Id, Name) VALUES(@ID, @NAME)",
                    sqlConnection);
                c.CommandTimeout = 15;
                c.Parameters.AddWithValue("@ID", id);
                c.Parameters.AddWithValue("@NAME", Name);

                c.ExecuteNonQuery();
                return GetRecipeCategory(id);
            }
        }

        public void DeleteRecipeCategory()
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var c = new SqlCommand("DELETE FROM RecipeCategory", sqlConnection);
                c.ExecuteNonQuery();
            }
        }

        public int GetNextIdRecipeCategory()
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                /* This query will return null, if the table is empty! */
                var c = new SqlCommand("SELECT MAX(Id) FROM RecipeCategory", sqlConnection);
                var obj = c.ExecuteScalar();
                if (obj is DBNull)
                    return 1;
                return (int) c.ExecuteScalar() + 1;
            }
        }
    }

    public class RecipeCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}