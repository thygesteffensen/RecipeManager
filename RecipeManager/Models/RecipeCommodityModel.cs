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

        public RecipeCommodity GetRecipeCommodity(Recipe recipe)
        {
            SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM RecipeCommodity WHERE " +
                $"RecipeID={recipe.Id}", _sqlConnection);

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                if (sqlDataReader.Read())
                {
                    RecipeCommodity recipeCommodity = new RecipeCommodity
                    {
                        recipe = recipe,
                        commodity = commodity,
                        Value = (int)sqlDataReader[0],
                        Unit = (string)sqlDataReader[1]
                    };

                    return recipeCommodity;
                }
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
        public Recipe recipe { get; set; }
        public Commodity commodity { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
    }

    public enum Units { kg, g, dl, L, stk, tsk, spsk };
}

