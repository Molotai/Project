using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuelMonitor.Model
{
    public class StationStatus
    {
        public int Id { get; set; }
        // StationId - is original id in oil network
        public int StationId { get; set; }
        
        // WOG, UPG, OKKO, etc
        public int NetworkId { get; set; }
        public string Status95Usual { get; set; }
        public string Status95Mod { get; set; }
        public string StatusGeneral { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
