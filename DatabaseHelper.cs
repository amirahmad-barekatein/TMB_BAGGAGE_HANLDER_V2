using System;
using System.Data.Odbc;
using ApiDataCollector.Models;
using System.Linq;

namespace ApiDataCollector
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTablesExist();
        }

        private void EnsureTablesExist()
        {
            using (var connection = new OdbcConnection(_connectionString))
            {
                connection.Open();

                // Create Flights table if it doesn't exist
                var createFlightsTableCommand = new OdbcCommand(@"
                    CREATE TABLE IF NOT EXISTS Flights (
                        FlightId VARCHAR(50) PRIMARY KEY,
                        Code VARCHAR(50),
                        AirlineTitle VARCHAR(100),
                        AirlineCode VARCHAR(50),
                        City VARCHAR(100),
                        AirportCode VARCHAR(50),
                        AirportTitle VARCHAR(100)
                    )", connection);
                createFlightsTableCommand.ExecuteNonQuery();

                // Create BaggageItems table if it doesn't exist
                var createBaggageItemsTableCommand = new OdbcCommand(@"
                    CREATE TABLE IF NOT EXISTS BaggageItems (
                        Barcode VARCHAR(50) PRIMARY KEY,
                        CreatedAt DATETIME,
                        GroupId VARCHAR(50),
                        CounterId VARCHAR(50),
                        Counter VARCHAR(50),
                        FlightId VARCHAR(50),
                        FOREIGN KEY (FlightId) REFERENCES Flights(FlightId)
                    )", connection);
                createBaggageItemsTableCommand.ExecuteNonQuery();
            }
        }

        public void SaveFlightData(FlightDataModel flightData)
        {
            using (var connection = new OdbcConnection(_connectionString))
            {
                connection.Open();

                // Insert or update flight information
                var flightCommand = new OdbcCommand(@"
                    UPDATE Flights SET Code = ?, AirlineTitle = ?, AirlineCode = ?, City = ?, AirportCode = ?, AirportTitle = ? WHERE FlightId = ?
                    IF @@ROWCOUNT = 0
                        INSERT INTO Flights (FlightId, Code, AirlineTitle, AirlineCode, City, AirportCode, AirportTitle) VALUES (?, ?, ?, ?, ?, ?, ?)", connection);

                flightCommand.Parameters.AddWithValue("@Code", flightData.Flight.Code);
                flightCommand.Parameters.AddWithValue("@AirlineTitle", flightData.Airline.Title);
                flightCommand.Parameters.AddWithValue("@AirlineCode", flightData.Airline.Code);
                flightCommand.Parameters.AddWithValue("@City", flightData.City);
                flightCommand.Parameters.AddWithValue("@AirportCode", flightData.Airport.Code);
                flightCommand.Parameters.AddWithValue("@AirportTitle", flightData.Airport.Title);
                flightCommand.Parameters.AddWithValue("@FlightId", flightData.Flight.Id);
                // Repeat parameters for the INSERT part
                flightCommand.Parameters.AddWithValue("@FlightId", flightData.Flight.Id);
                flightCommand.Parameters.AddWithValue("@Code", flightData.Flight.Code);
                flightCommand.Parameters.AddWithValue("@AirlineTitle", flightData.Airline.Title);
                flightCommand.Parameters.AddWithValue("@AirlineCode", flightData.Airline.Code);
                flightCommand.Parameters.AddWithValue("@City", flightData.City);
                flightCommand.Parameters.AddWithValue("@AirportCode", flightData.Airport.Code);
                flightCommand.Parameters.AddWithValue("@AirportTitle", flightData.Airport.Title);

                flightCommand.ExecuteNonQuery();

                // Insert baggage items
                foreach (var item in flightData.Baggage.Items)
                {
                    var baggageCommand = new OdbcCommand(@"
                        INSERT INTO BaggageItems (Barcode, CreatedAt, GroupId, CounterId, Counter, FlightId) 
                        VALUES (?, ?, ?, ?, ?, ?)
                        ON DUPLICATE KEY UPDATE CreatedAt = ?, GroupId = ?, CounterId = ?, Counter = ?", connection);

                    baggageCommand.Parameters.AddWithValue("@Barcode", item.Barcode);
                    baggageCommand.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
                    baggageCommand.Parameters.AddWithValue("@GroupId", item.Group);
                    baggageCommand.Parameters.AddWithValue("@CounterId", item.CounterId);
                    baggageCommand.Parameters.AddWithValue("@Counter", item.Counter);
                    baggageCommand.Parameters.AddWithValue("@FlightId", flightData.Flight.Id);
                    // Repeat parameters for the ON DUPLICATE KEY UPDATE part
                    baggageCommand.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
                    baggageCommand.Parameters.AddWithValue("@GroupId", item.Group);
                    baggageCommand.Parameters.AddWithValue("@CounterId", item.CounterId);
                    baggageCommand.Parameters.AddWithValue("@Counter", item.Counter);

                    baggageCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
