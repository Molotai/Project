using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuelMonitor.Model
{

    public class WogStationStatus
    {
        public int id { get; set; }
        public string? link { get; set; }
        public string city { get; set; }
        public string name { get; set; }
        //public Dictionary<string, Location>? coordinates { get; set; }
        public string? workDescription { get; set; }
    }

    public class Location
    {
        public string longitude { get; set; }
        public string latitude { get; set; }
    }
}
