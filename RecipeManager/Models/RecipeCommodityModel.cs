using System.Collections.Generic;
using System.Data.SqlClient;

namespace RecipeManager.Models
{
    internal class RecipeCommodityModel
    {
        private readonly string _dbPath;

        public RecipeCommodityModel(string dbPath)
        {
            _dbPath = dbPath;
        }

        public List<RecipeCommodity> GetRecipeCommodity(Recipe recipe)
        {
            var list = new List<RecipeCommodity>();

            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand("SELECT * FROM RecipeCommodity WHERE " +
                                                $"RecipeID={recipe.Id}", sqlConnection);
                var sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    while (sqlDataReader.Read())
                    {
                        var recipeCommodity = new RecipeCommodity
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

            var commodityModel = new CommodityModel(_dbPath);

            foreach (var recipeCommodity in list)
            {
                var commodity = commodityModel.GetCommodity(recipeCommodity.CommodityId);
                recipeCommodity.Commodity = commodity;
            }

            return list;
        }

        public void CreateRecipeCommodity(Recipe recipe, Commodity commodity, double Value, string Unit)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var c = new SqlCommand("INSERT INTO RecipeCommodity (RecipeID, CommodityID, Value, Unit) " +
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
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var c = new SqlCommand("DELETE FROM RecipeCommodity", sqlConnection);
                c.ExecuteNonQuery();
            }
        }

        public void DeleteRecipeCommodities(Recipe recipe)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var c = new SqlCommand($"DELETE FROM RecipeCommodity WHERE RecipeID={recipe.Id}", sqlConnection);
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
    }
}