using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    class RecipeCommodityModel
    {
        private readonly SqlConnection _sqlConnection;
        public RecipeCommodityModel(SqlConnection sqlConnection)
        {
            this._sqlConnection = sqlConnection;
        }

        public List<RecipeCommodity> GetRecipeCommodity(Recipe recipe)
        {
            CommodityModel commodityModel = new CommodityModel(_sqlConnection);
            SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM RecipeCommodity WHERE " +
                $"RecipeID={recipe.Id}", _sqlConnection);

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                List<RecipeCommodity> list = new List<RecipeCommodity>();

                while (sqlDataReader.Read())
                {
                    RecipeCommodity recipeCommodity = new RecipeCommodity
                    {
                        Recipe = recipe,
                        CommodityId = (int)sqlDataReader[1],
                        Value = (double)sqlDataReader[2],
                        Unit = (string)sqlDataReader[3]
                    };

                    list.Add(recipeCommodity);
                }
                sqlDataReader.Close();

                foreach (RecipeCommodity recipeCommodity in list)
                {
                    Commodity commodity = commodityModel.GetCommodity(recipeCommodity.CommodityId);
                    recipeCommodity.Commodity = commodity;
                }

                return list;
            }
            finally
            {
                sqlDataReader.Close();
            }
            return null;
        }

        public void CreateRecipeCommodity(Recipe recipe, Commodity commodity, double Value, string Unit)
        {
            SqlCommand c = new SqlCommand("INSERT INTO RecipeCommodity (RecipeID, CommodityID, Value, Unit) " +
                "VALUES(@RECIPEID, @COMMODITYID, @VALUE, @UNIT)", _sqlConnection);
            c.CommandTimeout = 15;

            c.Parameters.AddWithValue("@RECIPEID", recipe.Id);
            c.Parameters.AddWithValue("@COMMODITYID", commodity.Id);
            c.Parameters.AddWithValue("@VALUE", Value);
            c.Parameters.AddWithValue("@UNIT", Unit);

            c.ExecuteNonQuery();
        }

        public void DeleteRecipeCommodity()
        {
            SqlCommand c = new SqlCommand("DELETE FROM RecipeCommodity", _sqlConnection);
            c.ExecuteNonQuery();
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

    public enum Units { kg, g, dl, L, stk, tsk, spsk };
}

