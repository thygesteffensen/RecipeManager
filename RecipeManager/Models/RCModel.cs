using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    class RCModel
    {
        SqlConnection sqlConnection;
        public RCModel(SqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public List<RecipeToCategory> GetRecipes(RecipeCategory recipeCategory)
        {
            SqlCommand sqlCommand = new SqlCommand($"SELECT RecipeID FROM RC WHERE " +
                $"RecipeCategoryID={recipeCategory.Id}", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                RecipeModel recipeModel = new RecipeModel(sqlConnection);
                List<RecipeToCategory> recipeToCategories = new List<RecipeToCategory>();

                while (sqlDataReader.Read())
                {
                    RecipeToCategory recipeToCategory = new RecipeToCategory
                    {
                        recipe = recipeModel.GetRecipe((int)sqlDataReader[0]),
                        recipeCategory = recipeCategory
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

        public void DeleteRC()
        {
            SqlCommand c = new SqlCommand("DELETE FROM RC", sqlConnection);
            c.ExecuteNonQuery();
        }

        public void CreateRC(Recipe recipe, RecipeCategory recipeCategory)
        {
            SqlCommand c = new SqlCommand("INSERT INTO RC (RecipeID, ReciopeCategoryID) " +
                "VALUES(@RECIPEID, @RECIPECATEGORYID)", sqlConnection);
            c.CommandTimeout = 15;
            c.Parameters.AddWithValue("@COMMODITYID", recipe.Id);
            c.Parameters.AddWithValue("@COMMODITYCATEGORYID", recipeCategory.Id);

            c.ExecuteNonQuery();
        }
    }


    public class RecipeToCategory
    {
        public Recipe recipe { get; set; }
        public RecipeCategory recipeCategory { get; set; }
    }
}
