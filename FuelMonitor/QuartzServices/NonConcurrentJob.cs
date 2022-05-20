using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FuelMonitor.Services;
using System.Text.Json;
using Telegram;
using Telegram.Bot;
using FuelMonitor.Model;

namespace FuelMonitor.QuartzServices
{
    [DisallowConcurrentExecution]
    public class NonConconcurrentJob : IJob
    {
        private readonly ILogger<NonConconcurrentJob> _logger;
        private static int _counter = 0;
        private readonly IHubContext<JobsHub> _hubContext;
        private static Services.Monitor _monitor;

        public NonConconcurrentJob(ILogger<NonConconcurrentJob> logger,
               IHubContext<JobsHub> hubContext,
                Services.Monitor monitor)
        {
            _logger = logger;
            _hubContext = hubContext;
            _monitor = monitor;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var count = _counter++;
            List<string> apiUrls = new List<string>(new string[] 
            {
                "https://api.wog.ua/fuel_stations/839",
                "https://api.wog.ua/fuel_stations/945",
                "https://api.wog.ua/fuel_stations/1076",
                "https://api.wog.ua/fuel_stations/1077",
                "https://api.wog.ua/fuel_stations/934"
            });
            var beginMessage = $"Check {count} {DateTime.UtcNow} ";
            string before, now;
            await _hubContext.Clients.All.SendAsync("NonConcurrentJobs", beginMessage);
            _logger.LogInformation(beginMessage);

            using (WebClient wc = new WebClient())
            {
                var bot = new TelegramBotClient("2110311442:AAHF9iOD6r0Qrwq2bCmYkBFDou0il2vD410");

                //https://api.wog.ua/fuel_stations/839
                //https://api.wog.ua/fuel_stations/945
                // Бармаки
                //https://api.wog.ua/fuel_stations/1076
                //Дубенська
                //https://api.wog.ua/fuel_stations/1077
                foreach (var url in apiUrls)
                {
                    var json = wc.DownloadString(url);
                    int len = json.Length - 1 - 8;
                    json = json.Substring(8, len);
                    WogStationStatus? status = JsonSerializer.Deserialize<WogStationStatus>(json);

                    await _hubContext.Clients.All.SendAsync("TableContent", status, DateTime.UtcNow);

                    if (_monitor.isChangedM95(status.id, status.workDescription, out before, out now))
                    {
                        //found!
                        string statusChanged = $"{DateTime.UtcNow} " + status.name + " " + now + " Before: " + before;
                        await _hubContext.Clients.All.SendAsync("NonConcurrentJobs", statusChanged);
                        var t = await bot.SendTextMessageAsync("354114706", statusChanged);
                    }
                    /*
                    if (_monitor.isChangedA95(status.id, status.workDescription, out before, out now))
                    {
                        //found!
                        string statusChanged = $"{DateTime.UtcNow} " + status.name + " " + now + " Before: " + before;
                        await _hubContext.Clients.All.SendAsync("NonConcurrentJobs", statusChanged);
                        var t = await bot.SendTextMessageAsync("354114706", statusChanged);
                    }
                    */
                }
            }

            //var endMessage = $"NonConconcurrentJob Job END {count} {DateTime.UtcNow}";
            //await _hubContext.Clients.All.SendAsync("NonConcurrentJobs", endMessage);
            //_logger.LogInformation(endMessage);
        }
    }
}
