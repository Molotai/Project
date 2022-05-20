using FuelMonitor.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuelMonitor.Data
{
    public class FuelContext : DbContext
    {
        public FuelContext(DbContextOptions<FuelContext> options) : base(options)
        { 
        }
        public DbSet<StationStatus> Stations { get; set; }
    }
}
