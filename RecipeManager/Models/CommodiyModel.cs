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

                    Console.WriteLine($"{sqlDataReader[0]}, {sqlDataReader[1]}");
                    return commodity;
                }
            }
            finally
            {
                sqlDataReader.Close();
            }
            return null;
        }

        public Commodity CreateCommodity(string name)
        {
            int id = GetNextIdCommodity();
            SqlCommand c = new SqlCommand("INSERT INTO Commodity (Id, name) VALUES(@ID, @NAME)", sqlConnection);
            c.CommandTimeout = 15;
            c.Parameters.AddWithValue("@ID", id);
            c.Parameters.AddWithValue("@NAME", name);

            c.ExecuteNonQuery();

            return GetCommodity(id);
        }

        public void DeleteCommodity()
        {
            SqlCommand c = new SqlCommand("DELETE FROM Commodity", sqlConnection);
            c.ExecuteNonQuery();
        }

        public int GetNextIdCommodity()
        {
            SqlCommand c = new SqlCommand("SELECT MAX(Id) FROM Commodity", sqlConnection);
            object obj = c.ExecuteScalar();
            if (obj is System.DBNull)
            {
                return 1;
            }
            else
            {
                return (int)c.ExecuteScalar() + 1;
            }
        }
    }

    public class Commodity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
