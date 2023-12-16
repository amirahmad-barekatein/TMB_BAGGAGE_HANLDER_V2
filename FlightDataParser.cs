using Newtonsoft.Json;
using ApiDataCollector.Models;
using System.Collections.Generic;

namespace ApiDataCollector
{
    public static class FlightDataParser
    {
        public static List<FlightDataModel> ParseJson(string jsonData)
        {
            return JsonConvert.DeserializeObject<List<FlightDataModel>>(jsonData);
        }
    }
}
