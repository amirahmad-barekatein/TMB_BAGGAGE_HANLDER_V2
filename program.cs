using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ApiDataCollector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsetting.json", false, true)
                .Build();

            string apiUrl = configuration["ApiSettings:BaseUrl"];
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            var dataFetcher = new DataFetcher();
            //  Console.WriteLine(dataFetcher);
            var databaseHelper = new DatabaseHelper(connectionString);

            while (true)
            {
                try
                {
                    string jsonData = await dataFetcher.FetchDataFromApiAsync(apiUrl);
                    var flightData = FlightDataParser.ParseJson(jsonData);

                    foreach (var flight in flightData)
                    {
                        databaseHelper.SaveFlightData(flight);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                await Task.Delay(60000); // Delay for 1 minute
            }
        }
    }
}
