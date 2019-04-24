using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    class CommodiyModel
    {
        SqlConnection sqlConnection;
        public CommodiyModel(SqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public Commodity GetCommodity(int ID)
        {
            SqlCommand sqlCommand= new SqlCommand($"SELECT * FROM Commodity WHERE Id={ID}", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                if(sqlDataReader.Read())
                {
                    Commodity commodity = new Commodity
                    {
                        Id = (int)sqlDataReader[0],
                        Name = (string)sqlDataReader[1]
                    };

                    Console.WriteLine(String.Format("{0}, {1}", sqlDataReader[0], sqlDataReader[1]));
                    return commodity;
                }
            }
            finally
            {
                sqlDataReader.Close();
            }
            return null;
        }

        public void CreateCommodity(string Name)
        {
            SqlCommand c = new SqlCommand("INSERT INTO Commodity (Id, Name) VALUES(@ID, @NAME)", sqlConnection);
            c.CommandTimeout = 15;
            c.Parameters.AddWithValue("@ID", getNextIDCommodity()+1);
            c.Parameters.AddWithValue("@NAME", Name);

            c.ExecuteNonQuery();
        }

        public void DeleteCommodity()
        {
            SqlCommand c = new SqlCommand("DELETE FROM Commodity", sqlConnection);
            c.ExecuteNonQuery();
        }

        public int getNextIDCommodity()
        {
            SqlCommand c = new SqlCommand("SELECT MAX(Id) FROM Commodity", sqlConnection);
            return (int)c.ExecuteScalar();
        }
    }

    public class Commodity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
