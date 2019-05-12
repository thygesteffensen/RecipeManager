using System.Collections.Generic;
using System.Data.SqlClient;

namespace RecipeManager.Models
{
    class CCCModel
    {
        private readonly string _dbPath;

        public CCCModel(string dbPath)
        {
            this._dbPath = dbPath;
        }

        public List<CommodityToCategory> GetCommodities(CommodityCategory commodityCategory)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand($"SELECT CommodityID FROM CCC WHERE " +
                                                       $"CommodityCategoryID={commodityCategory.Id}", sqlConnection);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    CommodityModel commodiytModel = new CommodityModel(_dbPath);
                    List<CommodityToCategory> commodityToCategories = new List<CommodityToCategory>();

                    while (sqlDataReader.Read())
                    {
                        CommodityToCategory commodityToCategory = new CommodityToCategory
                        {
                            commodity = commodiytModel.GetCommodity((int) sqlDataReader[0]),
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
        }

        public void CreateCCC(Commodity commodity, CommodityCategory commodityCategory)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                SqlCommand c = new SqlCommand("INSERT INTO CCC (CommodityID, CommodityCategoryID) " +
                                              "VALUES(@COMMODITYID, @COMMODITYCATEGORYID)", sqlConnection);
                c.CommandTimeout = 15;
                c.Parameters.AddWithValue("@COMMODITYID", commodity.Id);
                c.Parameters.AddWithValue("@COMMODITYCATEGORYID", commodityCategory.Id);

                c.ExecuteNonQuery();
            }
        }

        public void DeleteCCC()
        {
            using (SqlConnection sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                SqlCommand c = new SqlCommand("DELETE FROM CCC", sqlConnection);
                c.ExecuteNonQuery();
            }
        }
    }


    public class CommodityToCategory
    {
        public Commodity commodity { get; set; }
        public CommodityCategory commodityCategory { get; set; }
    }
}