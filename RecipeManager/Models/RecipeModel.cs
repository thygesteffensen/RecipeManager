using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;
using System.Windows;

namespace RecipeManager.Models
{
    class RecipeModel
    {
        private readonly string _dbPath;

        public RecipeModel(string dbPath)
        {
            this._dbPath = dbPath;
        }

        public Recipe GetRecipe(int ID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM Recipe WHERE Id={ID}", sqlConnection);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    if (sqlDataReader.Read())
                    {
                        Recipe recipe = new Recipe
                        {
                            Id = (int) sqlDataReader[0],
                            Name = (string) sqlDataReader[1],
                            Description = (string) sqlDataReader[2]
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
        }

        public Recipe CreateRecipe(string Name, string Description)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                int id = getNextIDRecipes();
                SqlCommand c = new SqlCommand(
                    "INSERT INTO Recipe (Id, Name, Description) VALUES(@ID, @NAME, @DESCRIPTION)",
                    sqlConnection);
                c.CommandTimeout = 15;
                c.Parameters.AddWithValue("@ID", id);
                c.Parameters.AddWithValue("@NAME", Name);
                c.Parameters.AddWithValue("@DESCRIPTION", Description);

                c.ExecuteNonQuery();

                return GetRecipe((id));
            }
        }

        public List<Recipe> GetRecipes(RecipeCategory recipeCategory)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand =
                    new SqlCommand(
                        $"SELECT * FROM Recipe INNER JOIN RC ON Recipe.Id = RC.RecipeID WHERE RC.RecipeCategoryID = {recipeCategory.Id}",
                        sqlConnection);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    List<Recipe> recipes = new List<Recipe>();
                    while (sqlDataReader.Read())
                    {
                        Recipe recipe = new Recipe
                        {
                            Id = (int) sqlDataReader[0],
                            Name = (string) sqlDataReader[1],
                            Description = (string) sqlDataReader[2]
                        };

                        recipes.Add(recipe);
                    }

                    return recipes;
                }
                finally
                {
                    sqlDataReader.Close();
                }
            }
        }

        public void DeleteRecipes()
        {
            using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                SqlCommand c = new SqlCommand("DELETE FROM Recipe", sqlConnection);
                c.ExecuteNonQuery();
            }
        }

        public void DeleteRecipe(Recipe recipe)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                // Delete the recipes occurence in its category and its commodities
                RecipeCommodityModel recipeCommodityModel = new RecipeCommodityModel(_dbPath);
                recipeCommodityModel.DeleteRecipeCommodities(recipe);

                RCModel rcModel = new RCModel(_dbPath);
                rcModel.DeleteRC(recipe);

                using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
                {
                    sqlConnection.Open();
                    SqlCommand c = new SqlCommand($"DELETE FROM Recipe WHERE Id={recipe.Id}", sqlConnection);
                    c.ExecuteNonQuery();
                }

                scope.Complete();
            }
        }

        public void EditRecipe(Recipe recipe)
        {

        }

        public int getNextIDRecipes()
        {
            using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                SqlCommand c = new SqlCommand("SELECT MAX(Id) FROM Recipe", sqlConnection);
                object obj = c.ExecuteScalar();
                if (obj is System.DBNull)
                {
                    return 1;
                }
                else
                {
                    return (int) c.ExecuteScalar() + 1;
                }
            }
        }
    }

    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}