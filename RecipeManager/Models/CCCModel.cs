using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    class CCCModel
    {
        SqlConnection sqlConnection;
        public CCCModel(SqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public List<CommodityToCategory> GetComodities(CommodityCategory commodityCategory)
        {
            SqlCommand sqlCommand = new SqlCommand($"SELECT CommodityID FROM CCC WHERE " +
                $"CommodityCategoryID={commodityCategory.Id}", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                CommodiyModel commodiytModel = new CommodiyModel(sqlConnection);
                List<CommodityToCategory> commodityToCategories = new List<CommodityToCategory>();

                while (sqlDataReader.Read())
                {
                    CommodityToCategory commodityToCategory = new CommodityToCategory
                    {
                        commodity = commodiytModel.GetCommodity((int)sqlDataReader[0]),
                        commodityCategory = commodityCategory
                    };

                    commodityToCategories.Add(commodityToCategory);
                }

                return commodityToCategories;
            }
            finally
            {
                sqlDataReader.Close();
            }
        }

        public void CreateCCC(Commodity commodity, CommodityCategory commodityCategory)
        {
            SqlCommand c = new SqlCommand("INSERT INTO CCC (CommodityID, CommodityCategoryID) " +
                "VALUES(@COMMODITYID, @COMMODITYCATEGORYID)", sqlConnection);
            c.CommandTimeout = 15;
            c.Parameters.AddWithValue("@COMMODITYID", commodity.Id);
            c.Parameters.AddWithValue("@COMMODITYCATEGORYID", commodityCategory.Id);

            c.ExecuteNonQuery();
        }

        public void DeleteCCC()
        {
            SqlCommand c = new SqlCommand("DELETE FROM CCC", sqlConnection);
            c.ExecuteNonQuery();
        }
    }


    public class CommodityToCategory
    {
        public Commodity commodity { get; set; }
        public CommodityCategory commodityCategory{ get; set; }
    }
}

