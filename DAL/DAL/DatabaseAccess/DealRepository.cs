using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using DAL.Models;
using Dapper;

namespace DAL.DatabaseAccess
{
    // static repo xDD
    public static class DealRepository
    {
        private static readonly string _connectionString = "Server=localhost;Database=parser;User Id=sa;Password=mssql100@;";

        public static void SaveDeals(IList<Deal> deals)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = 
                    "IF NOT EXISTS(SELECT * FROM deals WHERE deal_number = @DealNumber) " + 
                    "INSERT INTO deals VALUES " + 
                    "(@DealNumber, @DealDate, @SellerName, @SellerInn, @BuyerName, @BuyerInn, @WoodVolumeSeller, @WoodVolumeBuyer)";
                
                foreach (var deal in deals)
                {
                    try
                    {
                        connection.Execute(query, deal);

                    }
                    catch (SqlTypeException e)
                    {
                        Console.WriteLine("Попалась строка с датой меньше 1753 года");
                    }
                }
            }
        }
    }
}