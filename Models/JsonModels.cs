using System;
using System.Collections.Generic;

namespace ApiDataCollector.Models
{
    public class FlightDataModel
    {
        public FlightInfoModel Flight { get; set; }
        public AirlineModel Airline { get; set; }
        public string City { get; set; }
        public AirportModel Airport { get; set; }
        public BaggageModel Baggage { get; set; }
    }

    public class FlightInfoModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
    }

    public class AirlineModel
    {
        public string Title { get; set; }
        public string Code { get; set; }
    }

    public class AirportModel
    {
        public string Code { get; set; }
        public string Title { get; set; }
    }

    public class BaggageModel
    {
        public Dictionary<string, int> Total { get; set; }
        public List<BaggageItemModel> Items { get; set; }
    }

    public class BaggageItemModel
    {
        public string Barcode { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Group { get; set; }
        public string CounterId { get; set; }
        public string Counter { get; set; }
    }
}
