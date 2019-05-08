using System.Collections.Generic;
using System.Data.SqlClient;

namespace RecipeManager.Models
{
    class RecipeCommodityModel
    {
        private readonly string _dbPath;

        public RecipeCommodityModel(string dbPath)
        {
            this._dbPath = dbPath;
        }

        public List<RecipeCommodity> GetRecipeCommodity(Recipe recipe)
        {
            List<RecipeCommodity> list = new List<RecipeCommodity>();

            using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM RecipeCommodity WHERE " +
                                                       $"RecipeID={recipe.Id}", sqlConnection);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    while (sqlDataReader.Read())
                    {
                        RecipeCommodity recipeCommodity = new RecipeCommodity
                        {
                            Recipe = recipe,
                            CommodityId = (int) sqlDataReader[1],
                            Value = (double) sqlDataReader[2],
                            Unit = (string) sqlDataReader[3]
                        };

                        list.Add(recipeCommodity);
                    }
                }
                finally
                {
                    sqlDataReader.Close();
                }
            }

            CommodityModel commodityModel = new CommodityModel(_dbPath);

            foreach (RecipeCommodity recipeCommodity in list)
            {
                Commodity commodity = commodityModel.GetCommodity(recipeCommodity.CommodityId);
                recipeCommodity.Commodity = commodity;
            }

            return list;
        }

        public void CreateRecipeCommodity(Recipe recipe, Commodity commodity, double Value, string Unit)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                SqlCommand c = new SqlCommand("INSERT INTO RecipeCommodity (RecipeID, CommodityID, Value, Unit) " +
                                              "VALUES(@RECIPEID, @COMMODITYID, @VALUE, @UNIT)", sqlConnection);
                c.CommandTimeout = 15;

                c.Parameters.AddWithValue("@RECIPEID", recipe.Id);
                c.Parameters.AddWithValue("@COMMODITYID", commodity.Id);
                c.Parameters.AddWithValue("@VALUE", Value);
                c.Parameters.AddWithValue("@UNIT", Unit);

                c.ExecuteNonQuery();
            }
        }

        public void DeleteAllRecipeCommodities()
        {
            using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                SqlCommand c = new SqlCommand("DELETE FROM RecipeCommodity", sqlConnection);
                c.ExecuteNonQuery();
            }
        }

        public void DeleteRecipeCommodities(Recipe recipe)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                SqlCommand c = new SqlCommand($"DELETE FROM RecipeCommodity WHERE RecipeID={recipe.Id}", sqlConnection);
                c.ExecuteNonQuery();
            }
        }
    }

    public class RecipeCommodity
    {
        public Recipe Recipe { get; set; }
        public Commodity Commodity { get; set; }
        public int CommodityId { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
    }

    public enum Units
    {
        kg,
        g,
        dl,
        L,
        stk,
        tsk,
        spsk
    };
}