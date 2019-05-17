using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RecipeManager.Models
{
    internal class CommodityModel
    {
        private readonly string _dbPath;

        public CommodityModel(string dbPath)
        {
            _dbPath = dbPath;
        }

        public Commodity GetCommodity(int ID)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand($"SELECT * FROM Commodity WHERE Id={ID}", sqlConnection);
                var sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    if (sqlDataReader.Read())
                    {
                        var commodity = new Commodity
                        {
                            Id = (int) sqlDataReader[0],
                            Name = (string) sqlDataReader[1]
                        };

                        return commodity;
                    }
                }
                finally
                {
                    sqlDataReader.Close();
                }

                return null;
            }
        }

        public List<Commodity> GetCommodities(string partialName = null)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand;
                if (partialName == null)
                    sqlCommand = new SqlCommand("SELECT * FROM Commodity", sqlConnection);
                else
                    sqlCommand = new SqlCommand($"SELECT * FROM Commodity WHERE Name LIKE '%{partialName}%'",
                        sqlConnection);

                var sqlDataReader = sqlCommand.ExecuteReader();
                try
                {
                    var commodities = new List<Commodity>();
                    while (sqlDataReader.Read())
                    {
                        var commodity = new Commodity
                        {
                            Id = (int) sqlDataReader[0],
                            Name = (string) sqlDataReader[1]
                        };

                        commodities.Add(commodity);
                    }

                    return commodities;
                }
                finally
                {
                    sqlDataReader.Close();
                }
            }
        }

        public Commodity CreateCommodity(string name)
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var id = GetNextIdCommodity();
                var c = new SqlCommand("INSERT INTO Commodity (Id, name) VALUES(@ID, @NAME)", sqlConnection);
                c.CommandTimeout = 15;
                c.Parameters.AddWithValue("@ID", id);
                c.Parameters.AddWithValue("@NAME", name);

                c.ExecuteNonQuery();

                return GetCommodity(id);
            }
        }

        public void DeleteCommodity()
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var c = new SqlCommand("DELETE FROM Commodity", sqlConnection);
                c.ExecuteNonQuery();
            }
        }

        public int GetNextIdCommodity()
        {
            using (var sqlConnection = new SqlConnection(_dbPath))
            {
                sqlConnection.Open();
                var c = new SqlCommand("SELECT MAX(Id) FROM Commodity", sqlConnection);
                var obj = c.ExecuteScalar();
                if (obj is DBNull)
                    return 1;
                return (int) c.ExecuteScalar() + 1;
            }
        }
    }

    public class Commodity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    ///     Used to shadow unknown commodities...
    /// </summary>
    public class CommodityShadow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Commodity Commodity { get; set; }
        public double Value { get; set; }
        public Units Unit { get; set; }
    }
}