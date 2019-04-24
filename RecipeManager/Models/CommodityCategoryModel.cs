using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    class CommodityCategoryModel
    {
        SqlConnection sqlConnection;
        public CommodityCategoryModel(SqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public CommodityCategory GetCommodityCategory(int ID)
        {
            SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM CommodityCategory WHERE Id={ID}", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                if (sqlDataReader.Read())
                {
                    CommodityCategory commodityCategory = new CommodityCategory
                    {
                        Id = (int)sqlDataReader[0],
                        Name = (string)sqlDataReader[1]
                    };

                    Console.WriteLine(String.Format("{0}, {1}", sqlDataReader[0], sqlDataReader[1]));
                    return commodityCategory;
                }
            }
            finally
            {
                sqlDataReader.Close();
            }
            return null;
        }

        public void CreateCommodityCategory(string Name)
        {
            SqlCommand c = new SqlCommand("INSERT INTO CommodityCategory (Id, Name) VALUES(@ID, @NAME)", sqlConnection);
            c.CommandTimeout = 15;
            c.Parameters.AddWithValue("@ID", getNextIDCommodityCategories() + 1);
            c.Parameters.AddWithValue("@NAME", Name);

            c.ExecuteNonQuery();
        }

        public void DeleteCommodityCategory()
        {
            SqlCommand c = new SqlCommand("DELETE FROM CommodityCategory", sqlConnection);
            c.ExecuteNonQuery();
        }

        public int getNextIDCommodityCategories()
        {
            SqlCommand c = new SqlCommand("SELECT MAX(Id) FROM CommodityCategory", sqlConnection);
            return (int)c.ExecuteScalar();
        }
    }


    public class CommodityCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
