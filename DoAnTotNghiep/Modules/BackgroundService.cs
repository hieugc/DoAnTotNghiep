using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Hubs;
using DoAnTotNghiep.ViewModels;
using Hangfire;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
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
                _timer = new Timer(ExecuteCheckIn, null, this._startTime, TimeSpan.FromDays(1));
            }
            else if (_function == (TargetFunction.ExecuteCheckIn))
            {
                _timer = new Timer(ExecuteCheckOut, null, this._startTime, TimeSpan.FromDays(1));
            }
            else if(_function == TargetFunction.ExecuteCheckTransaction)
            {
                _timer = new Timer(ExecuteCheckTransaction, null, this._startTime, TimeSpan.FromMinutes(1));
            }
            return Task.CompletedTask;
        }

        private void ExecuteCheckIn(object? Object)
        {
            //tới trước ngày đó thông báo sẽ Check-In
            //=> gửi signalR
            //=> gửi mail
            //=> timer kiểm tra người dùng đó
            if (this.executionCount < this._limit)
            {
                var count = Interlocked.Increment(ref executionCount);
                RequestBackground requestBackground = (RequestBackground)this._functionObject;
                if (count == 1)
                {
                    List<Notification> notificationList = new List<Notification>();
                    var userRequest = requestBackground.Request.Users;
                    var houseRequest = requestBackground.Request.Houses;
                    var swapHouseRequest = requestBackground.Request.SwapHouses;
                    User? userHouseRequest = null;

                    if (userRequest == null)
                    {
                        userRequest = this._context.Users.Where(m => m.Id == requestBackground.Request.IdUser).FirstOrDefault();
                    }

                    if (userRequest != null)
                    {
                        Notification notification = new Notification().Request(requestBackground.Request);
                        notification.Content = "Bạn có yêu cầu trao đổi vào lúc "
                                                    + requestBackground.Request.StartDate.ToString("dd/MM/yyyy")
                                                    + " - "
                                                    + requestBackground.Request.EndDate.ToString("dd/MM/yyyy")
                                                    + " cần Check In";
                        notification.Users = userRequest;
                        notificationList.Add(notification);
                    }
                    if (houseRequest == null)
                    {
                        houseRequest = this._context.Houses.Where(m => m.Id == requestBackground.Request.IdHouse).FirstOrDefault();
                    }

                    if (requestBackground.Request.Type == 2 && swapHouseRequest == null && requestBackground.Request.IdSwapHouse.HasValue)
                    {
                        swapHouseRequest = this._context.Houses.Where(m => m.Id == requestBackground.Request.IdSwapHouse.Value).FirstOrDefault();
                        if (houseRequest != null && userHouseRequest == null)
                        {
                            userHouseRequest = this._context.Users.Where(m => m.Id == houseRequest.IdUser).FirstOrDefault();
                        }
                        if (houseRequest != null && userHouseRequest != null)
                        {
                            Notification notification = new Notification().Request(requestBackground.Request);
                            notification.IdUser = userHouseRequest.Id;
                            notification.Content = "Bạn có yêu cầu trao đổi vào lúc "
                                                    + requestBackground.Request.StartDate.ToString("dd/MM/yyyy")
                                                    + " - "
                                                    + requestBackground.Request.EndDate.ToString("dd/MM/yyyy")
                                                    + " cần Check In";
                            notification.Users = userHouseRequest;
                            notificationList.Add(notification);
                        }
                    }

                    try
                    {
                        this._context.Notifications.AddRange(notificationList);
                        this._context.SaveChanges();

                        //=> gửi signalR
                        foreach (var item in notificationList)
                        {
                            requestBackground.ChatHub.SendNotification(
                                    group: Crypto.EncodeKey(item.IdUser.ToString(), Crypto.Salt(requestBackground.Configuration)),
                                    target: TargetSignalR.Notification(),
                                    model: new NotificationViewModel(item, this._host)).Wait(TimeSpan.FromMinutes(1));
                        }

                        //=> gửi mail
                        foreach (var item in notificationList)
                        {
                            if (item.Users != null)
                            {
                                string? moduleEmail = requestBackground.Configuration.GetConnectionString(ConfigurationEmail.Email());
                                string? modulePassword = requestBackground.Configuration.GetConnectionString(ConfigurationEmail.Password());
                                if (!string.IsNullOrEmpty(moduleEmail) && !string.IsNullOrEmpty(modulePassword))
                                {
                                    Email sender = new Email(moduleEmail, modulePassword);
                                    string body = item.Content;
                                    sender.SendMail(item.Users.Email, Subject.SendCheckIn(), body, null, string.Empty);
                                }
                            }
                        }

                        Console.WriteLine(count.ToString() + " Time: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));

                        this._timer?.Change(TimeSpan.FromDays(1), TimeSpan.FromDays(1));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        this.StopAsync(this._cancel);
                    }
                }
                else if (count == 2)
                {
                    var rq = this._context.Requests.Where(m => m.Id == requestBackground.Request.Id).FirstOrDefault();
                    if (rq != null)
                    {
                        if (rq.Status == (int)StatusRequest.ACCEPT) //chưa Check In
                        {
                            rq.Status = (int)StatusRequest.REJECT;
                            try
                            {
                                this._context.Requests.Update(rq);
                                this._context.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                    }
                    this.StopAsync(this._cancel);
                }
            }
            else
            {
                this.StopAsync(this._cancel);
            }
        }
        private void ExecuteCheckOut(object? Object)
        {
            //tới trước ngày đó thông báo sẽ Check-Out
            //=> gửi signalR
            //=> gửi mail
            if (this.executionCount < this._limit)
            {
                var count = Interlocked.Increment(ref executionCount);
                RequestBackground requestBackground = (RequestBackground)this._functionObject;
                List<Notification> notificationList = new List<Notification>();

                var userRequest = requestBackground.Request.Users;
                var houseRequest = requestBackground.Request.Houses;
                var swapHouseRequest = requestBackground.Request.SwapHouses;
                User? userHouseRequest = null;

                if (userRequest == null)
                {
                    userRequest = this._context.Users.Where(m => m.Id == requestBackground.Request.IdUser).FirstOrDefault();
                }

                if (userRequest != null)
                {
                    Notification notification = new Notification().Request(requestBackground.Request);
                    notification.Content = "Bạn có yêu cầu trao đổi vào lúc " + requestBackground.Request.StartDate.ToString("dd/MM/yyyy") + " - " + requestBackground.Request.EndDate.ToString("dd/MM/yyyy") + " cần Check Out";
                    notification.Users = userRequest;
                    notificationList.Add(notification);
                }
                if (houseRequest == null)
                {
                    houseRequest = this._context.Houses.Where(m => m.Id == requestBackground.Request.IdHouse).FirstOrDefault();
                }

                if (requestBackground.Request.Type == 2 && swapHouseRequest == null && requestBackground.Request.IdSwapHouse.HasValue)
                {
                    swapHouseRequest = this._context.Houses.Where(m => m.Id == requestBackground.Request.IdSwapHouse.Value).FirstOrDefault();
                    if (houseRequest != null && userHouseRequest == null)
                    {
                        userHouseRequest = this._context.Users.Where(m => m.Id == houseRequest.IdUser).FirstOrDefault();
                    }
                    if (houseRequest != null && userHouseRequest != null)
                    {
                        Notification notification = new Notification().Request(requestBackground.Request);
                        notification.IdUser = userHouseRequest.Id;
                        notification.Content = "Bạn có yêu cầu trao đổi vào lúc " + requestBackground.Request.StartDate.ToString("dd/MM/yyyy") + " - " + requestBackground.Request.EndDate.ToString("dd/MM/yyyy") + " cần Check Out";
                        notification.Users = userHouseRequest;
                        notificationList.Add(notification);
                    }
                }

                try
                {
                    this._context.Notifications.AddRange(notificationList);
                    this._context.SaveChanges();

                    //=> gửi signalR
                    foreach (var item in notificationList)
                    {
                        requestBackground.ChatHub.SendNotification(
                                group: Crypto.EncodeKey(item.IdUser.ToString(), Crypto.Salt(requestBackground.Configuration)),
                                target: TargetSignalR.Notification(),
                                model: new NotificationViewModel(item, this._host)).Wait(TimeSpan.FromMinutes(1));
                    }

                    //=> gửi mail
                    foreach (var item in notificationList)
                    {
                        if (item.Users != null)
                        {
                            string? moduleEmail = requestBackground.Configuration.GetConnectionString(ConfigurationEmail.Email());
                            string? modulePassword = requestBackground.Configuration.GetConnectionString(ConfigurationEmail.Password());
                            if (!string.IsNullOrEmpty(moduleEmail) && !string.IsNullOrEmpty(modulePassword))
                            {
                                Email sender = new Email(moduleEmail, modulePassword);
                                string body = item.Content;
                                sender.SendMail(item.Users.Email, Subject.SendCheckOut(), body, null, string.Empty);
                            }
                        }
                    }

                    Console.WriteLine(count.ToString() + " Time: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                this.StopAsync(this._cancel);
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
                                            ImageUrl = "/Image/dollar coin.svg"
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
}
