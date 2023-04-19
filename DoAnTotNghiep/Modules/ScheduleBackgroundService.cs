using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Hubs;
using DoAnTotNghiep.ViewModels;
using Hangfire;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;

namespace DoAnTotNghiep.Modules
{
    public class ScheduleBackgroundService : IHostedService, IDisposable
    {
        private Timer? _timer = null;
        private DoAnTotNghiepContext _context;

        public ScheduleBackgroundService(
                                    DoAnTotNghiepContext context)
        {
            _context = context;
        }
        public Task StartAsync(CancellationToken stoppingToken)
        {
            DateTime now = DateTime.Now;
            Console.WriteLine("Timed Hosted Service running. Time: " + now.ToString("dd/MM/yyyy hh:mm:ss"));
            DateTime timeRun = new DateTime(now.Year, now.Month, now.Day, 3, 0, 0, 0);
            if (DateTime.Compare(timeRun, now) < 0)
            {
                timeRun.AddDays(1);
            }
            TimeSpan timeToGo = timeRun - DateTime.Now;
            if (timeToGo <= TimeSpan.Zero)
            {
                timeToGo = TimeSpan.Zero;
            }

            _timer = new Timer(ExecuteSchedule, null, timeToGo, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void ExecuteSchedule(object? Object)
        {
            this.RemoveCircleSwap();
            this.CreateCircleSwap();
        }

        private void CreateCircleSwap()
        {
            // Lấy tất cả waiting
            //Định nghĩa status
            var waitingRq = this._context.WaitingRequests.ToList();
        }

        private void RemoveCircleSwap()
        {
            this.RemoveOutDate();
            this.RemoveDisable();
        }

        private void RemoveOutDate()
        {
            // Lấy tất cả waiting
            DateTime now = DateTime.Now;
            //xóa outdate
        }
        private void RemoveDisable()
        {
            var DBtransaction = this._context.Database.BeginTransaction();
            var DisableWaiting = this._context.WaitingRequests.Where(m => m.Status == (int)StatusWaitingRequest.DISABLE).ToList();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Timed Hosted Service is stopping. Time: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            _timer?.Change(Timeout.Infinite, 0);
            this.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
