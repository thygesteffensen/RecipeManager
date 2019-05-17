using System.Collections.Generic;
using System.Data.SqlClient;

namespace RecipeManager.Models
{
    internal class RCModel
    {
        private readonly string _dbPath;

        public RCModel(string dbPath)
        {
            _dbPath = dbPath;
        }

        public List<RecipeToCategory> GetRecipes(RecipeCategory recipeCategory)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand("SELECT RecipeID FROM RC WHERE " +
                                                $"RecipeCategoryID={recipeCategory.Id}", sqlConnection);
                var sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    var recipeModel = new RecipeModel(_dbPath);
                    var recipeToCategories = new List<RecipeToCategory>();

                    while (sqlDataReader.Read())
                    {
                        var recipeToCategory = new RecipeToCategory
                        {
                            Recipe = recipeModel.GetRecipe((int) sqlDataReader[0]),
                            RecipeCategory = recipeCategory
                        };

                        recipeToCategories.Add(recipeToCategory);
                    }

                    return recipeToCategories;
                }
                finally
                {
                    sqlDataReader.Close();
                }
            }
        }

        public RecipeToCategory GetCategory(Recipe recipe)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand("SELECT RecipeCategoryID FROM RC WHERE " +
                                                $"RecipeID={recipe.Id}", sqlConnection);
                var sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    var recipeCategoryModel = new RecipeCategoryModel(_dbPath);
                    var recipeToCategories = new List<RecipeToCategory>();

                    if (sqlDataReader.Read())
                    {
                        var recipeToCategory = new RecipeToCategory
                        {
                            Recipe = recipe,
                            RecipeCategory = recipeCategoryModel.GetRecipeCategory((int) sqlDataReader[0])
                        };

                        return recipeToCategory;
                    }
                }
                finally
                {
                    sqlDataReader.Close();
                }
            }

            return null;
        }

        public void DeleteAllRC()
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var c = new SqlCommand("DELETE FROM RC", sqlConnection);
                c.ExecuteNonQuery();
            }
        }

        public void DeleteRC(Recipe recipe)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var c = new SqlCommand($"DELETE FROM RC WHERE RecipeID={recipe.Id}", sqlConnection);
                c.ExecuteNonQuery();
            }
        }

        public RecipeToCategory CreateRC(Recipe recipe, RecipeCategory recipeCategory)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var c = new SqlCommand("INSERT INTO RC (RecipeID, RecipeCategoryID) " +
                                       "VALUES(@RECIPEID, @RECIPECATEGORYID)", sqlConnection);
                c.CommandTimeout = 15;
                c.Parameters.AddWithValue("@RECIPEID", recipe.Id);
                c.Parameters.AddWithValue("@RECIPECATEGORYID", recipeCategory.Id);

                c.ExecuteNonQuery();

                return new RecipeToCategory
                {
                    Recipe = recipe,
                    RecipeCategory = recipeCategory
                };
            }
        }
    }


    public class RecipeToCategory
    {
        public Recipe Recipe { get; set; }
        public RecipeCategory RecipeCategory { get; set; }
    }
}