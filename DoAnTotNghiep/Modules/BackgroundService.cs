﻿using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Hubs;
using DoAnTotNghiep.Service;
using DoAnTotNghiep.ViewModels;
using Hangfire;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
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
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private Timer? _timer = null;
        private int _limit = 1;

        private CancellationToken _cancel = CancellationToken.None;
        private DoAnTotNghiepContext _context;
        private string _host;
        private string _function;
        private TimeSpan _startTime;
        private object _functionObject;

        public TimedHostedService(
                                    DoAnTotNghiepContext context,
                                    string host,
                                    string function,
                                    CancellationToken cancellationToken,
                                    int limit,
                                    TimeSpan startTime,
                                    object functionObject)
        {
            _limit = limit;
            _cancel = cancellationToken;
            _context = context;
            _host = host;
            _function = function;
            _startTime = startTime;
            _functionObject = functionObject;
        }
        public Task StartAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Timed Hosted Service running. Time: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            if(_function == (TargetFunction.ExecuteCheckIn))
            {
                _timer = new Timer(ExecuteCheckIn, null, this._startTime, TimeSpan.FromHours(40));
            }
            else if (_function == (TargetFunction.ExecuteCheckIn))
            {
                _timer = new Timer(ExecuteCheckOut, null, this._startTime, TimeSpan.FromHours(40));
            }
            else if(_function == TargetFunction.ExecuteCheckTransaction)
            {
                _timer = new Timer(ExecuteCheckTransaction, null, this._startTime, TimeSpan.FromSeconds(10));
            }
            else if (_function == TargetFunction.ExecuteCreateWaiting)
            {
                _timer = new Timer(ExecuteCreateWaiting, null, this._startTime, TimeSpan.FromMinutes(5));
            }
            else if (_function == TargetFunction.ExecuteCheckInCircleRequest)
            {
                _timer = new Timer(ExecuteCheckInCircleRequest, null, this._startTime, TimeSpan.FromMinutes(1));
            }
            else if (_function == TargetFunction.ExecuteCheckOutCircleRequest)
            {
                _timer = new Timer(ExecuteCheckOutCircleRequest, null, this._startTime, TimeSpan.FromMinutes(1));
            }
            return Task.CompletedTask;
        }
        //request
        private void ExecuteCheckIn(object? Object)
        {
            if (this.executionCount < this._limit)
            {
                var count = Interlocked.Increment(ref executionCount);
                RequestBackground requestBackground = (RequestBackground)this._functionObject;

                List<Notification> notificationList = new List<Notification>();
                var userRequest = requestBackground.Request.Users;
                var houseRequest = requestBackground.Request.Houses;
                var swapHouseRequest = requestBackground.Request.SwapHouses;//này để làm gì

                if (userRequest == null)
                {
                    userRequest = this._context.Users.Where(m => m.Id == requestBackground.Request.IdUser).FirstOrDefault();
                }
                if (houseRequest == null)
                {
                    houseRequest = this._context.Houses.Where(m => m.Id == requestBackground.Request.IdHouse).FirstOrDefault();
                }
                if (count == 1)
                {
                    notificationList.AddRange(
                        this.CreateNotification(userRequest,  
                                                houseRequest, 
                                                requestBackground,
                                                (requestBackground.Request.Type == 2 && requestBackground.Request.IdSwapHouse.HasValue),
                                                " cần check-in")
                    );
                    var DBtransaction = this._context.Database.BeginTransaction();

                    try
                    {
                        this._context.Notifications.AddRange(notificationList);
                        this._context.SaveChanges();
                        Console.WriteLine(count.ToString() + " Time: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        DBtransaction.Commit();
                        this.SendNotificationAndMail(notificationList, requestBackground, Subject.SendCheckIn());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        this.StopAsync(this._cancel);
                        DBtransaction.Rollback();
                    }
                }
                else if (count == 2)
                {
                    var rq = this._context.Requests
                                        .Include(m => m.Houses).ThenInclude(h => h.Users)
                                        .Where(m => m.Id == requestBackground.Request.Id).FirstOrDefault();
                    var DBtransaction = this._context.Database.BeginTransaction();
                    if (rq != null)
                    {
                        if (rq.Status == (int)StatusRequest.ACCEPT) //chưa Check In
                        {
                            notificationList.AddRange(
                                this.CreateNotification(userRequest,
                                                houseRequest,
                                                requestBackground,
                                                true,
                                                " đã được hệ thống tự check-in")
                            );

                            //rq.Status = (int)StatusRequest.REJECT;
                            rq.Status = (int)StatusRequest.CHECK_IN; //tự động CheckIn

                            try
                            {
                                this._context.Notifications.AddRange(notificationList);
                                this._context.SaveChanges();

                                this._context.Requests.Update(rq);
                                this._context.SaveChanges();
                                DBtransaction.Commit();
                                this.SendNotificationAndMail(notificationList, requestBackground, Subject.SendCheckIn());

                                this._limit += 1;
                                TimeSpan timeToGo = rq.EndDate.AddDays(-1) - DateTime.Now;
                                if (timeToGo <= TimeSpan.Zero)
                                {
                                    timeToGo = TimeSpan.Zero;
                                }
                                this._timer?.Change(timeToGo, TimeSpan.FromMinutes(1));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                DBtransaction.Rollback();
                            }
                        }
                        else
                        {
                            this.StopAsync(this._cancel);
                        }
                    }
                    else
                    {
                        this.StopAsync(this._cancel);
                    }
                }
                else if (count == 3)
                {
                    this.ExecuteCheckOutAlert(notificationList, userRequest, houseRequest, requestBackground, count);
                }
            }
            else
            {
                this.StopAsync(this._cancel);
            }
        }
        private void ExecuteCheckOutAlert(List<Notification> notificationList, User? userRequest, House? houseRequest, RequestBackground requestBackground, int count)
        {
            notificationList.AddRange(
                        this.CreateNotification(userRequest,
                                                houseRequest,
                                                requestBackground,
                                                (requestBackground.Request.Type == 2 && requestBackground.Request.IdSwapHouse.HasValue),
                                                " cần check-out"));
            var DBtransaction = this._context.Database.BeginTransaction();
            try
            {
                this._context.Notifications.AddRange(notificationList);
                this._context.SaveChanges();
                DBtransaction.Commit();
                this.SendNotificationAndMail(notificationList, requestBackground, Subject.SendCheckOut());

                Console.WriteLine(count.ToString() + " Time: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DBtransaction.Rollback();
            }
            this.StopAsync(this._cancel);
        }
        private List<Notification> CreateNotification(User? userRequest, House? houseRequest, RequestBackground requestBackground, bool condition, string content)
        {
            List<Notification> notificationList = new List<Notification>();

            if (userRequest != null)
            {
                Notification notification = new Notification().Request(requestBackground.Request);
                notification.Content = "Bạn có yêu cầu trao đổi vào lúc "
                                            + requestBackground.Request.StartDate.ToString("dd/MM/yyyy")
                                            + " - "
                                            + requestBackground.Request.EndDate.ToString("dd/MM/yyyy")
                                            + content;
                notificationList.Add(notification);
            }
            if (condition)
            {
                if (houseRequest != null)
                {
                    Notification notification = new Notification().Request(requestBackground.Request);
                    notification.IdUser = houseRequest.IdUser;
                    notification.Content = "Bạn có yêu cầu trao đổi vào lúc "
                                            + requestBackground.Request.StartDate.ToString("dd/MM/yyyy")
                                            + " - "
                                            + requestBackground.Request.EndDate.ToString("dd/MM/yyyy")
                                            + content;
                    notificationList.Add(notification);
                }
            }
            return notificationList;
        }
        private void SendNotificationAndMail(List<Notification> notifications, RequestBackground requestBackground, string title)
        {
            foreach (var item in notifications)
            {
                requestBackground.ChatHub.SendNotification(
                        group: Crypto.EncodeKey(item.IdUser.ToString(), Crypto.Salt(requestBackground.Configuration)),
                        target: TargetSignalR.Notification(),
                        model: new NotificationViewModel(item, this._host)).Wait(TimeSpan.FromMinutes(1));
            }

            foreach (var item in notifications)
            {
                var u = this._context.Users.FirstOrDefault(m => m.Id == item.IdUser);
                if (u != null)
                {
                    string? moduleEmail = requestBackground.Configuration.GetConnectionString(ConfigurationEmail.Email());
                    string? modulePassword = requestBackground.Configuration.GetConnectionString(ConfigurationEmail.Password());
                    if (!string.IsNullOrEmpty(moduleEmail) && !string.IsNullOrEmpty(modulePassword))
                    {
                        EmailSender sender = new EmailSender(moduleEmail, modulePassword);
                        string body = item.Content;
                        sender.SendMail(item.Users.Email, title, body, null, string.Empty);
                    }
                }
            }
        }
        private void SendNotificationAndMail(List<Notification> notifications, CircleRequestBackground requestBackground, string title)
        {
            foreach (var item in notifications)
            {
                requestBackground.ChatHub.SendNotification(
                        group: Crypto.EncodeKey(item.IdUser.ToString(), Crypto.Salt(requestBackground.Configuration)),
                        target: TargetSignalR.Notification(),
                        model: new NotificationViewModel(item, this._host)).Wait(TimeSpan.FromMinutes(1));
            }

            foreach (var item in notifications)
            {
                var u = this._context.Users.FirstOrDefault(m => m.Id == item.IdUser);
                if (u != null)
                {
                    string? moduleEmail = requestBackground.Configuration.GetConnectionString(ConfigurationEmail.Email());
                    string? modulePassword = requestBackground.Configuration.GetConnectionString(ConfigurationEmail.Password());
                    if (!string.IsNullOrEmpty(moduleEmail) && !string.IsNullOrEmpty(modulePassword))
                    {
                        EmailSender sender = new EmailSender(moduleEmail, modulePassword);
                        string body = item.Content;
                        sender.SendMail(item.Users.Email, title, body, null, string.Empty);
                    }
                }
            }
        }
        private void ExecuteCheckOut(object? Object)
        {
            if (this.executionCount < this._limit)
            {
                var count = Interlocked.Increment(ref executionCount);
                RequestBackground requestBackground = (RequestBackground)this._functionObject;
                List<Notification> notificationList = new List<Notification>();

                var userRequest = requestBackground.Request.Users;
                var houseRequest = requestBackground.Request.Houses;

                if (userRequest == null)
                {
                    userRequest = this._context.Users.Where(m => m.Id == requestBackground.Request.IdUser).FirstOrDefault();
                }
                if (houseRequest == null)
                {
                    houseRequest = this._context.Houses.Where(m => m.Id == requestBackground.Request.IdHouse).FirstOrDefault();
                }
                this.ExecuteCheckOutAlert(notificationList, userRequest, houseRequest, requestBackground, count);
            }
            else
            {
                this.StopAsync(this._cancel);
            }
        }
        private void ExecuteCheckTransaction(object? Object)
        {
            CheckTransactionBackground checkObject = (CheckTransactionBackground)this._functionObject;
            if (this.executionCount < this._limit)
            {
                var count = Interlocked.Increment(ref executionCount);

                if(DateTime.Compare(DateTime.Now, checkObject.EndTime) < 0)
                {
                    try
                    {
                        string? result = string.Empty;
                        HttpClient client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, checkObject.Url);
                        request.Content = checkObject.Content;
                        HttpResponseMessage response = client.Send(request);

                        if (response.IsSuccessStatusCode)
                        {
                            result = response.Content.ReadAsStringAsync().Result;
                        }

                        ZaloResponseCheckOrder? zaloResponse = JsonConvert.DeserializeObject<ZaloResponseCheckOrder>(result);
                        if(zaloResponse != null)
                        {
                            if (zaloResponse.returncode == 1)
                            {
                                HistoryTransaction? transaction = this._context
                                                                        .HistoryTransactions
                                                                        .Where(m => m.IdUser == checkObject.Transaction.IdUser
                                                                                    && m.Id == checkObject.Transaction.Id
                                                                                    && m.Status == (int) StatusTransaction.PENDING)
                                                                        .FirstOrDefault();
                                if(transaction != null)
                                {
                                    var BDTransaction = this._context.Database.BeginTransaction();

                                    transaction.Status = (int)StatusTransaction.VALID;
                                    try
                                    {
                                        this._context.HistoryTransactions.Update(transaction);
                                        this._context.SaveChanges();

                                        var userTransaction = transaction.Users;

                                        if (userTransaction == null)
                                        {
                                            userTransaction = this._context.Users.Where(m => m.Id == transaction.IdUser).FirstOrDefault();
                                        }
                                        if (userTransaction != null)
                                        {
                                            userTransaction.Point += transaction.Amount;
                                            this._context.Users.Update(userTransaction);
                                            this._context.SaveChanges();
                                        }


                                        //signalR => notification
                                        Notification notification = new Notification()
                                        {
                                            Content = "Bạn đã nạp tiền thành công",
                                            CreatedDate = DateTime.Now,
                                            Title = "Nạp tích lũy",
                                            Type = NotificationType.PAYMENT,
                                            IdType = transaction.Id,
                                            IdUser = transaction.IdUser,
                                            IsSeen = false,
                                            ImageUrl = NotificationImage.Coin
                                        };
                                        this._context.Notifications.Add(notification);
                                        this._context.SaveChanges();


                                        checkObject.ChatHub.SendNotification(
                                            Crypto.EncodeKey(transaction.IdUser.ToString(), Crypto.Salt(checkObject.Configuration)),
                                            TargetSignalR.Notification(), 
                                            new NotificationViewModel(notification, this._host))
                                        .Wait(TimeSpan.FromMinutes(5));

                                        BDTransaction.Commit();
                                    }
                                    catch(Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                        BDTransaction.Rollback();
                                    }
                                }
                                this.StopAsync(this._cancel);
                                DeleteTransaction(this._context, checkObject.Transaction.Id, checkObject.Transaction.CreatedDate);
                            }
                            else if (zaloResponse.returncode == -54)
                            {
                                checkObject.ChatHub.SendPaymentError(
                                                        Crypto.EncodeKey(checkObject.Transaction.IdUser.ToString(), Crypto.Salt(checkObject.Configuration)),
                                                        TargetSignalR.Payment(),
                                                        "Giao dịch hết hạn")
                                                    .Wait(TimeSpan.FromMinutes(5));
                                //xóa luôn
                                this.StopAsync(this._cancel);
                                DeleteTransaction(this._context, checkObject.Transaction.Id, checkObject.Transaction.CreatedDate);
                            }
                        }      
                        //Console.WriteLine(count.ToString() + " Result: " + result + " Time: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        this.StopAsync(this._cancel);
                        DeleteTransaction(this._context, checkObject.Transaction.Id, checkObject.Transaction.CreatedDate);
                    }
                }
                else
                {
                    this.StopAsync(this._cancel);
                    DeleteTransaction(this._context, checkObject.Transaction.Id, checkObject.Transaction.CreatedDate);
                    checkObject.ChatHub.SendPaymentError(Crypto.EncodeKey(checkObject.Transaction.IdUser.ToString(), Crypto.Salt(checkObject.Configuration)),
                                                        TargetSignalR.Payment(),
                                                        "Giao dịch hết hạn")
                                                    .Wait(TimeSpan.FromMinutes(5));
                }
            }
            else
            {
                this.StopAsync(this._cancel);
                DeleteTransaction(this._context, checkObject.Transaction.Id, checkObject.Transaction.CreatedDate);
                checkObject.ChatHub.SendPaymentError(Crypto.EncodeKey(checkObject.Transaction.IdUser.ToString(), Crypto.Salt(checkObject.Configuration)),
                                                        TargetSignalR.Payment(),
                                                        "Giao dịch hết hạn")
                                                    .Wait(TimeSpan.FromMinutes(5));
            }
        }
        private void DeleteTransaction(DoAnTotNghiepContext context, int IdUser, DateTime time)
        {
            var transaction = context.HistoryTransactions.Where(m => m.IdUser == IdUser && m.Status == (int)StatusTransaction.PENDING && DateTime.Compare(m.CreatedDate, time) < 0).ToList();
            context.HistoryTransactions.RemoveRange(transaction);
            context.SaveChanges();
        }

        private void ExecuteCreateWaiting(object? Object)
        {
            //chạy 1 lần
            CreateWaitingRequest model = (CreateWaitingRequest)this._functionObject;
            var user = this._context.Users.Where(m => m.Id == model.IdUser).FirstOrDefault();
            if (user != null)
            {
                var waitingRq = this._context.WaitingRequests.Where(m => m.IdUser == model.IdUser && m.IdCity == model.IdCity).ToList();
                if (waitingRq.Count() > 0)
                {
                    if(model.DateStart != null && model.DateEnd != null)
                    {
                        foreach(var item in waitingRq)
                        {
                            item.StartDate = model.DateStart;
                            item.EndDate = model.DateEnd;
                        }
                        try
                        {
                            this._context.WaitingRequests.UpdateRange(waitingRq);
                            this._context.SaveChanges();
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
                else
                {
                    //kiểm tra nhà => có thì thêm
                    var houses = this._context.Houses
                                            .Where(m => m.IdUser == model.IdUser 
                                                    && m.Status == (int)StatusHouse.VALID 
                                                    && m.IdCity != m.IdCity)
                                            .ToList();
                    if(houses.Count() > 0)
                    {
                        //create
                        List<WaitingRequest> waitingRequest  = new List<WaitingRequest>();
                        foreach(var item in houses)
                        {
                            waitingRequest.Add(new WaitingRequest().CreateModel(model, item.Id));
                        }
                        try
                        {
                            this._context.WaitingRequests.AddRange(waitingRequest);
                            this._context.SaveChanges();
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
            this.StopAsync(this._cancel);
        }

        private void ExecuteCheckInCircleRequest(object? Object)
        {
            if (this.executionCount < this._limit)
            {
                var count = Interlocked.Increment(ref executionCount);
                CircleRequestBackground requestBackground = (CircleRequestBackground)this._functionObject;
                //thông báo checkIn
                //checkIn
                    //chưa check => check => alert checkout
                    //
                if (count == 1)
                {
                    List<Notification> notifications = this.CreateNotiForCircle(requestBackground.IdCircle, " cần check in");                    
                    var DBtransaction = this._context.Database.BeginTransaction();

                    try
                    {
                        this._context.Notifications.AddRange(notifications);
                        this._context.SaveChanges();
                        Console.WriteLine(count.ToString() + " Time: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        DBtransaction.Commit();
                        this.SendNotificationAndMail(notifications, requestBackground, Subject.SendCheckIn());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        this.StopAsync(this._cancel);
                        DBtransaction.Rollback();
                    }
                }
                else if (count == 2)
                {
                    var circleRequest = this._context.CircleExchangeHouses.FirstOrDefault(m => m.Id == requestBackground.IdCircle && m.Status == (int)StatusWaitingRequest.ACCEPT);
                    if(circleRequest != null)//chưa accept full
                    {
                        List<Notification> notifications = this.CreateNotiForCircle(requestBackground.IdCircle, " đã được hệ thống check in");

                        circleRequest.Status = (int)StatusWaitingRequest.CHECK_IN; //tự động CheckIn
                        var DBtransaction = this._context.Database.BeginTransaction();
                        try
                        {
                            this._context.Notifications.AddRange(notifications);
                            this._context.SaveChanges();

                            this._context.CircleExchangeHouses.Update(circleRequest);
                            this._context.SaveChanges();
                            DBtransaction.Commit();
                            this.SendNotificationAndMail(notifications, requestBackground, Subject.SendCheckIn());

                            this._limit += 1;
                            TimeSpan timeToGo = circleRequest.EndDate.AddDays(-1) - DateTime.Now;
                            if (timeToGo <= TimeSpan.Zero)
                            {
                                timeToGo = TimeSpan.Zero;
                            }
                            this._timer?.Change(timeToGo, TimeSpan.FromMinutes(1));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            DBtransaction.Rollback();
                        }
                    }
                    else
                    {
                        this.StopAsync(this._cancel);
                    }
                }
                else if (count == 3)
                {
                    List<Notification> notifications = this.CreateNotiForCircle(requestBackground.IdCircle, " cần check out");
                    var DBtransaction = this._context.Database.BeginTransaction();
                    try
                    {
                        this._context.Notifications.AddRange(notifications);
                        this._context.SaveChanges();
                        DBtransaction.Commit();
                        this.SendNotificationAndMail(notifications, requestBackground, Subject.SendCheckOut());
                        Console.WriteLine(count.ToString() + " Time: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        DBtransaction.Rollback();
                    }
                    this.StopAsync(this._cancel);
                }
            }
            else
            {
                this.StopAsync(this._cancel);
            }
        }
        private void ExecuteCheckOutCircleRequest(object? Object)
        {
            if (this.executionCount < this._limit)
            {
                var count = Interlocked.Increment(ref executionCount);
                CircleRequestBackground requestBackground = (CircleRequestBackground)this._functionObject;
                List<Notification> notifications = this.CreateNotiForCircle(requestBackground.IdCircle, " cần check out");
                var DBtransaction = this._context.Database.BeginTransaction();
                try
                {
                    this._context.Notifications.AddRange(notifications);
                    this._context.SaveChanges();
                    DBtransaction.Commit();
                    this.SendNotificationAndMail(notifications, requestBackground, Subject.SendCheckOut());
                    Console.WriteLine(count.ToString() + " Time: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    DBtransaction.Rollback();
                }
                this.StopAsync(this._cancel);
            }
            else
            {
                this.StopAsync(this._cancel);
            }
        }

        private List<Notification> CreateNotiForCircle(int idCircle, string content)
        {
            List<Notification> notifications = new List<Notification>();
            List<User> users = new List<User>();

            var itemU = from cr in this._context.CircleExchangeHouses
                        join cru in this._context.CircleExchangeHouseOfUsers on cr.Id equals cru.IdCircleExchangeHouse
                        join u in this._context.Users on cru.IdUser equals u.Id
                        where cr.Id == idCircle
                        select u;
            if (itemU != null) users.AddRange(itemU.ToList());
            List<WaitingRequest> requests = new List<WaitingRequest>();
            var itemRQ = from rcr in this._context.RequestsInCircleExchangeHouses
                         join wr in this._context.WaitingRequests on rcr.IdWaitingRequest equals wr.Id
                         where rcr.IdCircleExchangeHouse == idCircle
                         select wr;
            if (itemRQ != null) requests.AddRange(itemRQ.ToList());
            foreach (var i in requests) i.IncludeAll(this._context);
            foreach (var item in users)
            {
                WaitingRequest myRequest = requests.First(m => m.IdUser == item.Id);
                WaitingRequest nextRequest = requests.First(m => m.IdCity == myRequest.IdCity);

                Notification notification = new Notification()
                {
                    Title = NotificationType.RequestTitle,
                    CreatedDate = DateTime.Now,
                    Type = NotificationType.CIRCLE_SWAP,
                    IdUser = item.Id,
                    IdType = idCircle,
                    IsSeen = false,
                    ImageUrl = NotificationImage.Alert,
                    Content = "Yêu cầu của bạn đến nhà " + nextRequest.Houses.Name + " của " + nextRequest.Users.LastName + " " + nextRequest.Users.FirstName + " ở " + nextRequest.Citys.Name + content
                };
                notifications.Add(notification);
            }
            return notifications;
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

    public class RequestBackground
    {
        public ChatHub ChatHub { get; set; }
        public Request Request { get; set; }
        public IConfiguration Configuration { get; set; } 

        public RequestBackground(ChatHub chatHub, Request request, IConfiguration configuration)
        {
            ChatHub = chatHub;
            Request = request;
            Configuration = configuration;
        }
    }
    public class CircleRequestBackground
    {
        public ChatHub ChatHub { get; set; }
        public int IdCircle { get; set; }
        public IConfiguration Configuration { get; set; }

        public CircleRequestBackground(ChatHub chatHub, int idCircle, IConfiguration configuration)
        {
            ChatHub = chatHub;
            this.IdCircle = idCircle;
            Configuration = configuration;
        }
    }
    public class CheckTransactionBackground
    {
        public ChatHub ChatHub { get; set; }
        public Uri Url { get; set; }
        public FormUrlEncodedContent Content { get; set; }
        public DateTime EndTime { get; set; }
        public IConfiguration Configuration { get; set; }
        public HistoryTransaction Transaction { get; set; }
        public CheckTransactionBackground(
            Uri Url, 
            FormUrlEncodedContent Content, 
            DateTime endTime, 
            ChatHub chatHub, 
            IConfiguration Configuration, 
            HistoryTransaction Transaction)
        {
            this.Url = Url;
            this.Content = Content;
            EndTime = endTime;
            ChatHub = chatHub;
            this.Configuration = Configuration;
            this.Transaction = Transaction;
        }
    }
    public class CreateWaitingRequest
    {
        public int IdCity { get; set; }
        public int IdUser { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public CreateWaitingRequest(
            int IdCity,
            int IdUser,
            DateTime? DateStart,
            DateTime? DateEnd)
        {
            this.IdCity = IdCity;
            this.IdUser = IdUser;
            this.DateEnd = DateEnd;
            this.DateStart = DateStart;
        }
    }
}
