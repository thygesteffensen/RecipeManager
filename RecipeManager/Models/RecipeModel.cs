using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;

namespace RecipeManager.Models
{
    internal class RecipeModel
    {
        private readonly string _dbPath;

        public RecipeModel(string dbPath)
        {
            _dbPath = dbPath;
        }

        public Recipe GetRecipe(int ID)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand($"SELECT * FROM Recipe WHERE Id={ID}", sqlConnection);
                var sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    if (sqlDataReader.Read())
                    {
                        var recipe = new Recipe
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
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var id = getNextIDRecipes();
                var c = new SqlCommand(
                    "INSERT INTO Recipe (Id, Name, Description) VALUES(@ID, @NAME, @DESCRIPTION)",
                    sqlConnection);
                c.CommandTimeout = 15;
                c.Parameters.AddWithValue("@ID", id);
                c.Parameters.AddWithValue("@NAME", Name);
                c.Parameters.AddWithValue("@DESCRIPTION", Description);

                c.ExecuteNonQuery();

                return GetRecipe(id);
            }
        }

        public List<Recipe> GetRecipes(RecipeCategory recipeCategory)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var sqlCommand =
                    new SqlCommand(
                        $"SELECT * FROM Recipe INNER JOIN RC ON Recipe.Id = RC.RecipeID WHERE RC.RecipeCategoryID = {recipeCategory.Id}",
                        sqlConnection);
                var sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    var recipes = new List<Recipe>();
                    while (sqlDataReader.Read())
                    {
                        var recipe = new Recipe
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
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var c = new SqlCommand("DELETE FROM Recipe", sqlConnection);
                c.ExecuteNonQuery();
            }
        }

        public void DeleteRecipe(Recipe recipe)
        {
            using (var scope = new TransactionScope())
            {
                // Delete the recipes occurence in its category and its commodities
                var recipeCommodityModel = new RecipeCommodityModel(_dbPath);
                recipeCommodityModel.DeleteRecipeCommodities(recipe);

                var rcModel = new RCModel(_dbPath);
                rcModel.DeleteRC(recipe);

                using (var sqlConnection = new SqlConnection(_dbPath))
                {
                    sqlConnection.Open();
                    var c = new SqlCommand($"DELETE FROM Recipe WHERE Id={recipe.Id}", sqlConnection);
                    c.ExecuteNonQuery();
                }

                scope.Complete();
            }
        }

        public void EditRecipe(Recipe recipe, string recipeName, string recipeDescription)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var c = new SqlCommand(
                    $@"UPDATE Recipe SET Name='{recipeName}', Description='{recipeDescription}' WHERE Id={recipe.Id}",
                    sqlConnection);
                c.ExecuteNonQuery();
            }
        }

        public int getNextIDRecipes()
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var c = new SqlCommand("SELECT MAX(Id) FROM Recipe", sqlConnection);
                var obj = c.ExecuteScalar();
                if (obj is DBNull)
                    return 1;
                return (int) c.ExecuteScalar() + 1;
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