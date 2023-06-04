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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Drawing;
using NuGet.Protocol;
using System.ComponentModel;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.Service;

namespace DoAnTotNghiep.Job
{
    public class ScheduleBackgroundService : IHostedService, IDisposable
    {
        private Timer? _timer = null;
        private CircleRequestService _circleRequestService;
        private IConfiguration _configuration;
        

        public ScheduleBackgroundService(IConfiguration configuration)
        {
            var data = new DoAnTotNghiepContext(configuration);
            _circleRequestService = new CircleRequestService(data);
            _configuration = configuration;
        }
        public Task StartAsync(CancellationToken stoppingToken)
        {
            DateTime now = DateTime.Now;
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
            FileSystem.WriteExceptionFile("startSchedule in " + now.ToString("hh:mm:ss dd/MM/yyyy"), "startSchedule_" + now.ToString("hh_mm_ss_dd_MM_yyyy"));
            _timer = new Timer(ExecuteSchedule, null, timeToGo, TimeSpan.FromMinutes(5));
            //_timer = new Timer(ExecuteSchedule, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }
        private void ExecuteSchedule(object? Object)
        {
            this._circleRequestService.RemoveCircleSwap();
            this._circleRequestService.RemoveRequestOutDate();
            this.CreateCircleSwap();
        }
        private void CreateCircleSwap(int maxSize = 5)
        {
            List<WaitingRequestForSearch> requestSearch = this._circleRequestService.GetInitWaitingRequest();
            List<List<WaitingRequestForSearch>> requests = new List<List<WaitingRequestForSearch>>();
            int minSize = 3;
            while(!(requests.Count > 0 || minSize > maxSize || minSize > requestSearch.Count())) // dừng khi tìm đc vòng hoặc minSize == maxSize hoặc minSize > requestSearch.count
            {
                //list đã được order theo ToIdCity => duyệt list 1 lần
                for(int index = 0; index < requestSearch.Count(); index++)
                {
                    List<WaitingRequestForSearch> listBase = requestSearch.GetRange(index + 1, requestSearch.Count() - (index + 1));
                    List<WaitingRequestForSearch> head = new List<WaitingRequestForSearch>() { requestSearch.ElementAt(index) };
                    Recursive(listBase, minSize, head, requests);
                }
                minSize++;
            }
            if(requests.Count() > 0)
            {
                this._circleRequestService.Save(this.MakeUnique(this._circleRequestService.IsExist(requests)));
            }
        }
        private List<List<WaitingRequestForSearch>> MakeUnique(List<List<WaitingRequestForSearch>> requests)
        {
            List<List<WaitingRequestForSearch>> model = new List<List<WaitingRequestForSearch>>();
            foreach(var item in requests)
            {
                if (model.Count() == 0) model.Add(item);
                else
                {
                    bool isExist = false;
                    foreach (var mitem in model)
                    {
                        if(mitem.Count() == item.Count())
                        {
                            List<WaitingRequestForSearch> l_1 = mitem.OrderBy(m => m.IdUser).ToList();
                            List<WaitingRequestForSearch> l_2 = item.OrderBy(m => m.IdUser).ToList();
                            for(var index = 0; index < l_1.Count(); index++)
                            {
                                if (l_1[index].IdUser == l_2[index].IdUser
                                    && l_1[index].ToIdCity == l_2[index].ToIdCity
                                    && l_1[index].FromIdCity == l_2[index].FromIdCity
                                    && l_1[index].StartDate == l_2[index].StartDate
                                    && l_1[index].EndDate == l_2[index].EndDate)
                                {
                                    isExist = true;
                                }
                                else
                                {
                                    isExist = false;
                                }
                            }
                            if (isExist)
                            {
                                break;
                            }
                        }
                    }

                    if (!isExist) model.Add(item);
                }
            }

            return model;
        }
        private void Recursive(List<WaitingRequestForSearch> listBase, int size, List<WaitingRequestForSearch> prev, List<List<WaitingRequestForSearch>> res)
        {
            //điều kiện dừng
            if (size > prev.Count())
            {
                WaitingRequestForSearch tail = prev.Last();
                if (tail.StartDate != null && tail.EndDate != null)
                {
                    var search = listBase.FindAll(m => m.FromIdCity == tail.ToIdCity
                                                && !prev.Any(u => u.IdUser == m.IdUser)
                                                && (m.StartDate == null || m.EndDate == null ||
                                                m.StartDate != null && m.EndDate != null
                                                && !(DateTime.Compare(m.EndDate.Value, tail.StartDate.Value) < 0
                                                        || DateTime.Compare(tail.EndDate.Value, m.StartDate.Value) < 0)));
                    foreach (var item in search)
                    {
                        Recursive(listBase, size, this.GetList(item, prev, tail), res);
                    }
                }
                else
                {
                    var search = listBase.FindAll(m => m.FromIdCity == tail.ToIdCity && !prev.Any(u => u.IdUser == m.IdUser));
                    foreach (var item in search)
                    {
                        Recursive(listBase, size, this.GetList(item, prev, tail), res);
                    }
                }
            }
            else
            {
                WaitingRequestForSearch last = prev.Last();
                if (prev.First().FromIdCity == last.ToIdCity && last.StartDate != null && last.StartDate != null)
                {
                    res.Add(prev);
                }
            }
        }
        private List<WaitingRequestForSearch> GetList(WaitingRequestForSearch item, List<WaitingRequestForSearch> prev, WaitingRequestForSearch tail)
        {
            DateTime? date_1 = tail.StartDate;

            if (item.StartDate.HasValue &&
                (date_1 == null || date_1.HasValue && DateTime.Compare(date_1.Value, item.StartDate.Value) < 0))
            {
                date_1 = item.StartDate;
            }

            DateTime? date_2 = tail.EndDate;

            if (item.EndDate.HasValue &&
                (date_2 == null || date_2.HasValue && DateTime.Compare(item.EndDate.Value, date_2.Value) < 0))
            {
                date_2 = item.EndDate;
            }

            item.StartDate = date_1;
            item.EndDate = date_2;
            //tạo ra 1 cái list
            List<WaitingRequestForSearch> node = new List<WaitingRequestForSearch>();
            node.AddRange(prev);
            node.Add(item);
            return node;
        }
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            this.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

    public class WaitingRequestForSearch
    {
        public WaitingRequestForSearch(WaitingRequest request, int ToIdCity, int FromIdCity, DateTime? StartDate, DateTime? EndDate) {
            this.Request = request;
            this.ToIdCity = ToIdCity;
            this.FromIdCity = FromIdCity;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.IdUser = request.IdUser;
        }
        public WaitingRequest Request { get; set; }
        public int ToIdCity { get; set; }
        public int FromIdCity { get; set; }
        public int IdUser { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ObjectCircleAdd
    {
        public ObjectCircleAdd(CircleExchangeHouse circleExchangeHouse, List<WaitingRequest> waitingRequests)
        {
            this.CircleExchanges = circleExchangeHouse;
            WaitingRequests = waitingRequests;
        }
        public CircleExchangeHouse CircleExchanges { get; set; }
        public List<WaitingRequest> WaitingRequests { get; set; }
    }
}
