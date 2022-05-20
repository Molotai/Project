using FuelMonitor.Data;
using FuelMonitor.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuelMonitor.Services
{
    //{"data":{"link":"https://api.wog.ua/fuel_stations/934","city":"Kruhle","coordinates":{"longitude":26.354648,"latitude":50.608643},"workDescription":"АЗК працює згідно графіку.\nМ95 - Пальне відсутнє.\nА95 - тільки спецтранспорт.\nДП - тільки спецтранспорт.\nМДП+ - Пальне відсутнє.\nГАЗ - Готівка, банк.картки 20л. Гаманець ПРАЙД до 100л. Талони до 40л. Паливна картка (ліміт картки).\n","fuels":[{"cla":"#5d71b1","brand":"Євро5","name":"ДП","id":9},{ "cla":"#cf5c72","brand":"Mustang+","name":"ДП","id":6},{ "cla":"#5d71b1","brand":"Євро5","name":"95","id":2},{ "cla":"#cf5c72","brand":"Mustang","name":"95","id":5},{ "cla":"#4484ff","name":"ГАЗ","id":11}],"services":[{"icon":"vilka","name":"Розетка","id":1},{ "icon":"wogpride","name":"WOG Pride","id":2},{ "icon":"tirefitting","name":"Підкачка шин","id":4},{ "icon":"parking","name":"Паркінг","id":5},{ "icon":"wc","name":"Туалет","id":6},{ "icon":"wifi","name":"WI-FI","id":7},{ "icon":"wogcafe","name":"WOG Cafe","id":8},{ "icon":"wogmarket","name":"WOG Market","id":9},{ "icon":"charge","name":"Зарядка","id":10},{ "icon":"baby","name":"Пеленальні столики","id":11},{ "icon":"ibox","name":"IBox","id":13}],"schedule":[{"day":"Сьогодні","interval":"00:10 - 23:50"}],"name":"Rivne Region, on the territory of Bilokrynytska Village Coun","id":934}}

    public class Monitor
    {
        private FuelContext _context;
        public static Dictionary<int, string> M95Status;
        public static Dictionary<int, string> A95Status;
        public Monitor(FuelContext context)
        {
            _context = context;
        }

        public bool isChangedM95(int id, string bigStatus, out string before, out string now)
        {
            string[] words = bigStatus.Split('\n');
            bool rez = false;
            before = "";
            now = "";
            foreach (var word in words)
            {
                if (word.Contains("М95"))
                {
                    now = word;

                    StationStatus station =
                    _context.Stations.Where(p => p.StationId == id).FirstOrDefault();
                    if (station == null) //add to database
                    {
                        station = new StationStatus();
                        station.StationId = id;
                        station.Status95Mod = now;
                        station.StatusGeneral = bigStatus;
                        station.UpdateTime = DateTime.UtcNow;
                        _context.Stations.Add(station);
                    }
                    else // check for change
                    {
                        before = station.Status95Mod;
                        if (before != now)
                        {
                            rez = true;
                            station.Status95Mod = now;
                            station.StatusGeneral = bigStatus;
                            station.UpdateTime = DateTime.UtcNow;
                            _context.Entry(station).State = EntityState.Modified;
                        }
                    }
                    _context.SaveChanges();
                }
                else if (word.Contains("А95"))
                {
                    now = word;

                    StationStatus station =
                    _context.Stations.Where(p => p.StationId == id).FirstOrDefault();
                    if (station == null) //add to database
                    {
                        station = new StationStatus();
                        station.StationId = id;
                        station.Status95Usual = now;
                        station.StatusGeneral = bigStatus;
                        station.UpdateTime = DateTime.UtcNow;
                        _context.Stations.Add(station);
                    }
                    else // check for change
                    {
                        before = station.Status95Usual;
                        if (before != now)
                        {
                            rez = true;
                            station.Status95Usual = now;
                            station.StatusGeneral = bigStatus;
                            station.UpdateTime = DateTime.UtcNow;
                            _context.Entry(station).State = EntityState.Modified;
                        }
                    }
                    _context.SaveChanges();
                }
            }
            
            return rez;
        }

        public bool isChangedA95(int id, string bigStatus, out string before, out string now)
        {
            string[] words = bigStatus.Split('\n');
            bool rez = false;
            before = "";
            now = "";
            foreach (var word in words)
            {
                if (word.Contains("А95"))
                {
                    now = word;
                    if (A95Status.TryGetValue(id, out before))
                    {
                        if (before != now)
                        {
                            rez = true;
                        }
                        A95Status[id] = now;
                    }
                    else // no such key
                    {
                        A95Status.Add(id, now);
                        rez = true;
                    }
                }
            }
            return rez;
        }

        public Monitor()
        {
            M95Status = new Dictionary<int, string>();
            A95Status = new Dictionary<int, string>();
        }
    }
}
