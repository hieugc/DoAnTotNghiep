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

namespace DoAnTotNghiep.Job
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
            this.RemoveRequestOutDate();
        }

        private void CreateCircleSwap(int maxSize = 5)
        {
            //Lấy tất cả waiting vừa khởi tạo và trong vòng nhưng chưa xác nhận
            var waitingRq = this._context.WaitingRequests
                                        .Include(m => m.Houses)
                                        .Where(m => (m.Status == (int)StatusWaitingRequest.INIT) 
                                                        && m.Houses != null
                                                        && m.Houses.IdCity != null)
                                        .Select(m => new WaitingRequestForSearch(m, m.IdCity, m.Houses.IdCity.Value, m.StartDate, m.EndDate))
                                        .ToList();//có id

            List<WaitingRequestForSearch> requestSearch = new List<WaitingRequestForSearch>();
            requestSearch.AddRange(waitingRq.Where(m => m != null).ToList());
            DateTime now = DateTime.Now;
            var userRq = this._context.Requests
                                        .Include(m => m.Houses)
                                        .Include(m => m.SwapHouses)
                                        .Where(m => (m.Status == (int)StatusRequest.WAIT_FOR_SWAP || m.Status == (int)StatusRequest.REJECT)
                                                    && DateTime.Compare(now, m.EndDate) < 0 
                                                    && m.IdSwapHouse != null && m.Type == 2 
                                                    && m.SwapHouses != null 
                                                    && m.Houses != null
                                                    && m.SwapHouses.IdCity != null
                                                    && m.Houses.IdCity != null)
                                        .Select(m => new WaitingRequest().CreateModelByRequest(m))
                                        .ToList();//không có id
            if (userRq != null)
            {
                requestSearch.AddRange(userRq.Where(m => m != null).ToList());
            }
            requestSearch = requestSearch.OrderBy(m => m.ToIdCity).ThenBy(m => m.StartDate).ToList();

            List<List<WaitingRequestForSearch>> requests = new List<List<WaitingRequestForSearch>>();
            int minSize = 3;
            while(!(requests.Count > 0 || minSize > maxSize || minSize > requestSearch.Count())) // dừng khi tìm đc vòng hoặc minSize == maxSize hoặc minSize > requestSearch.count
            {
                //list đã được order theo ToIdCity => duyệt list 1 lần
                for(int index = 0; index < requestSearch.Count(); index++)
                {
                    List<WaitingRequestForSearch> listBase = new List<WaitingRequestForSearch>();
                    requestSearch.CopyTo(index + 1, listBase.ToArray(), 0, listBase.Count() - (index + 1));
                    List<WaitingRequestForSearch> head = new List<WaitingRequestForSearch>() { requestSearch.ElementAt(index) };
                    Recursive(listBase, minSize, head, requests);
                }
                minSize++;
            }

            var DBtransaction = this._context.Database.BeginTransaction();

            try
            {
                //có cái danh sách rồi thì lưu lại 
                List<ObjectCircleAdd> objectCircleAdds= new List<ObjectCircleAdd>();
                //List<WaitingRequest> newWaitingNode = new List<WaitingRequest>();
                //List<WaitingRequest> oldWaitingNode = new List<WaitingRequest>();
                foreach (var item in requests)
                {
                    if(item.Count() >= 3)
                    {
                        var listObj = item.Select(m => m.Request).ToList();
                        //danh sách trong 1 vòng
                        var newWaitingNode = listObj.Where(m => m.Id == 0).ToList();
                        this._context.WaitingRequests.AddRange(newWaitingNode);//thêm request mới => thêm do từ request -> và đã incircle

                        var oldWaitingNode = listObj.Where(m => m.Id != 0).ToList();
                        foreach (var ud in oldWaitingNode) ud.Status = (int)StatusWaitingRequest.IN_CIRCLE;
                        this._context.WaitingRequests.UpdateRange(oldWaitingNode);//update Status
                        this._context.SaveChanges();
                        CircleExchangeHouse circleExchange = new CircleExchangeHouse() { Status = (int)StatusWaitingRequest.INIT };
                        this._context.CircleExchangeHouses.AddRange(circleExchange);
                        this._context.SaveChanges();
                        objectCircleAdds.Add(new ObjectCircleAdd(circleExchange, listObj));
                    }
                }

                //this._context.SaveChanges();
                Console.WriteLine(objectCircleAdds.ToJson());//check lại mấy cái request có update id ch?

                foreach(var item in objectCircleAdds)
                {
                    List<CircleExchangeHouseOfUser> newUser = new List<CircleExchangeHouseOfUser>();
                    List<RequestInCircleExchangeHouse> newLink = new List<RequestInCircleExchangeHouse>();
                    foreach (var u in item.WaitingRequests)
                    {
                        newUser.Add(new CircleExchangeHouseOfUser() { IdCircleExchangeHouse = item.CircleExchanges.Id, IdUser = u.IdUser });
                        newLink.Add(new RequestInCircleExchangeHouse() { IdCircleExchangeHouse = item.CircleExchanges.Id, IdWaitingRequest = u.Id });
                    }
                    this._context.CircleExchangeHouseOfUsers.AddRange(newUser);
                    this._context.RequestsInCircleExchangeHouses.AddRange(newLink);
                }
                this._context.SaveChanges();

                DBtransaction.Commit();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                DBtransaction.Rollback();
            }
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
                        Recursive(listBase, size, this.GetList(item, prev, tail), res);//sao thiếu 3 + 5
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
                if (prev.First().FromIdCity == prev.Last().ToIdCity)
                {
                    WaitingRequestForSearch last = prev.Last();
                    foreach(var item in prev)
                    {
                        item.Request.StartDate = last.StartDate;
                        item.Request.EndDate = last.EndDate;
                    }
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


        private void RemoveCircleSwap()
        {
            // Lấy tất cả waiting
            DateTime now = DateTime.Now;
            //xóa outdate
            var rm = from cr in this._context.CircleExchangeHouses
                     join rcr in this._context.RequestsInCircleExchangeHouses on cr.Id equals rcr.IdCircleExchangeHouse
                     join wr in this._context.WaitingRequests on rcr.IdWaitingRequest equals wr.Id
                     where wr.Status == (int)StatusWaitingRequest.IN_CIRCLE && wr.StartDate != null && DateTime.Compare(wr.StartDate.Value, now) < 0
                     select cr;
            foreach (var item in rm) item.Status = (int)StatusWaitingRequest.DISABLE;
            try
            {
                this._context.CircleExchangeHouses.UpdateRange(rm);
                this._context.SaveChanges();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void RemoveRequestOutDate()
        {
            DateTime now = DateTime.Now;
            var request = this._context.Requests.Where(m => m.Status == (int)StatusRequest.WAIT_FOR_SWAP && DateTime.Compare(m.StartDate, now) < 0).ToList();
            foreach(var item in request)
            {
                item.Status = (int)StatusRequest.DISABLE;
            }

            try
            {
                this._context.Requests.UpdateRange(request);
                this._context.SaveChanges();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
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


    public class Test
    {
        public Test() { }
        public Test(int FromIdCity, int ToIdCity, string content, DateTime? dateStart, DateTime? dateEnd)
        {
            this.ToIdCity = ToIdCity;
            this.FromIdCity = FromIdCity;
            this.Content = content;
            this.StartDate = dateStart;
            this.EndDate = dateEnd;
        }
        public int ToIdCity { get; set; }
        public int FromIdCity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Content { get; set; }
        public string Text()
        {
            return "---ToIdCity: " + this.ToIdCity.ToString() + " FromIdCity: " + this.FromIdCity.ToString() + " Content: " + this.Content + " StartDate: " + this.StartDate?.ToString("dd/MM/yyyy") + " EndDate: " + this.EndDate?.ToString("dd/MM/yyyy") + "---";
        }
        private void Recursive(List<Test> listBase, int size, List<Test> prev, List<List<Test>> res)
        {
            //điều kiện dừng
            if (size > prev.Count())
            {
                Test tail = prev.Last();
                if (tail.StartDate != null && tail.EndDate != null)
                {
                    var search = listBase.FindAll(m => m.FromIdCity == tail.ToIdCity 
                                                && (m.StartDate == null || m.EndDate == null || 
                                                m.StartDate != null && m.EndDate != null 
                                                && !(DateTime.Compare(m.EndDate.Value, tail.StartDate.Value) < 0 
                                                        || DateTime.Compare(tail.EndDate.Value, m.StartDate.Value) < 0)));
                    foreach (var item in search)
                    {
                        Recursive(listBase, size, this.GetList(item, prev, tail), res);//sao thiếu 3 + 5
                    }
                }
                else
                {
                    var search = listBase.FindAll(m => m.FromIdCity == tail.ToIdCity);
                    foreach (var item in search)
                    {
                        Recursive(listBase, size, this.GetList(item, prev, tail), res);//sao thiếu 3 + 5
                    }
                }
            }
            else
            {
                if(prev.First().FromIdCity == prev.Last().ToIdCity)
                {
                    res.Add(prev);
                }
            }
        }
        private List<Test> GetList(Test item, List<Test> prev, Test tail)
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
            List<Test> node = new List<Test>();
            node.AddRange(prev);
            node.Add(item);
            return node;
        }
        public void TestRecursive()
        {
            List<Test> model = new List<Test>()
            {
                new Test(1, 2, "A", null, null),
                new Test(2, 4, "B", null, null),
                new Test(2, 3, "C", null, null),
                new Test(3, 5, "D", null, null),
                new Test(4, 5, "E", null, null),
                new Test(5, 6, "F", null, null),
                new Test(5, 7, "G", null, null),
                new Test(5, 8, "H", null, null),
                new Test(5, 9, "K", null, null)
            };

            int count = 0;
            List<List<Test>> res = new List<List<Test>>();
            for(int index = 0; index < model.Count(); index++)
            {
                List<Test> input = new List<Test>() { model.ElementAt(index) };
                List<Test> listBase = model.GetRange(index + 1 , model.Count() - (index + 1));

                Recursive(listBase, 3, input, res);
            }

            foreach (var item in res)
            {
                Console.WriteLine("-----------------Frame " + (count++).ToString() + "-----------------");

                foreach (var t in item)
                {
                    Console.WriteLine(t.Text());
                }
                Console.WriteLine("-----------------End Frame-----------------");
            }

            Console.WriteLine("-----------------End Test-----------------");
        }
        public void TestRecursiveWithTime()
        {
            List<Test> model = new List<Test>()
            {
                new Test(1, 2, "A", null, null),
                new Test(2, 4, "B", DateTime.Now.AddDays(1), DateTime.Now.AddDays(3)),
                new Test(2, 3, "C", null, null),
                new Test(3, 5, "D", null, null),
                new Test(4, 5, "E", null, null),
                new Test(5, 6, "F", null, null),
                new Test(5, 7, "G", null, null),
                new Test(5, 8, "H", null, null),
                new Test(5, 9, "K", null, null),
                new Test(9, 1, "N", null, null)
            };

            int count = 0;
            List<List<Test>> res = new List<List<Test>>();
            for (int index = 0; index < model.Count(); index++)
            {
                List<Test> input = new List<Test>() { model.ElementAt(index) };
                List<Test> listBase = model.GetRange(index + 1, model.Count() - (index + 1));

                Recursive(listBase, 5, input, res);
            }

            foreach (var item in res)
            {
                Console.WriteLine("-----------------Frame " + (count++).ToString() + "-----------------");

                foreach (var t in item)
                {
                    Console.WriteLine(t.Text());
                }
                Console.WriteLine("-----------------End Frame-----------------");
            }

            Console.WriteLine("-----------------End Test-----------------");
        }
    }
}
