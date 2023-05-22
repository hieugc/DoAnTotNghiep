using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using DoAnTotNghiep.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using QRCoder;
using System.Drawing;
using DoAnTotNghiep.Service;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Member)]
    public class HistoryTransactionController : BaseController
    {
        private readonly ILogger<HistoryTransactionController> _logger;
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private IHubContext<ChatHub> _signalContext;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public HistoryTransactionController(ILogger<HistoryTransactionController> logger, 
                    DoAnTotNghiepContext context, 
                    IConfiguration configuration, 
                    IHostEnvironment environment, 
                    IHubContext<ChatHub> signalContext,
                    ITransactionService transactionService,
                    IUserService userService,
                    INotificationService notificationService) : base(environment)
        {
            _logger = logger;
            this._context = context;
            this._configuration = configuration;
            _signalContext = signalContext;
            _transactionService = transactionService;
            _userService = userService;
            _notificationService = notificationService;
        }

        [HttpGet("/Payment/Form")]
        public IActionResult TransactionForm()
        {
            return PartialView("./Views/HistoryTransaction/_FormCreateTransaction.cshtml", new CreateTransaction());
        }

        [HttpPost("/Payment/Zalo")]
        public async Task<IActionResult> CreateOrderZaloAsync([FromBody] CreateTransaction amount)
        {
            if (ModelState.IsValid)
            {
                return await this.CreateAsync(amount, true);
            }
            return BadRequest(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
        [HttpPost("/api/Payment/Zalo")]
        public async Task<IActionResult> ApiCreateOrderZaloAsync([FromBody] CreateTransaction amount)
        {
            if (ModelState.IsValid)
            {
                return await this.CreateAsync(amount, false);
            }
            return BadRequest(new {
                Status = 400,
                Message = this.ModelErrors()
            });
        }

        private async Task<IActionResult> CreateAsync(CreateTransaction amount, bool isWeb)
        {
            string? key1 = this._configuration.GetConnectionString(ZaloPay.ZaloKey1);
            string? AppId = this._configuration.GetConnectionString(ZaloPay.ZaloId);

            if (!string.IsNullOrEmpty(key1) && !string.IsNullOrEmpty(AppId))
            {
                if (ModelState.IsValid)
                {
                    int IdUser = this.GetIdUser();
                    HistoryTransaction transaction = new HistoryTransaction()
                    {
                        CreatedDate = DateTime.Now,
                        Amount = amount.Price,
                        Status = (int)StatusTransaction.PENDING,
                        IdUser = IdUser,
                        Content = "Bạn đã nạp "
                                    + amount.Price
                                    + " VNĐ vào hệ thống tích lũy điểm lúc "
                                    + DateTime.Now.ToString("hh:mm:ss dd/MM/yyyy")
                    };
                    using (var tran = this._context.Database.BeginTransaction())
                    {
                        try
                        {
                            var user = this._userService.GetById(IdUser);
                            if (user != null)
                            {
                                this._transactionService.Save(transaction);
                                tran.Commit();

                                Uri url = RequestAPI.CreateOrderZaloRequest();
                                Dictionary<string, string> model = new ZaloViewModel().CreateOrder(user, transaction, this.GetWebsitePath(), key1, AppId);

                                var response = await RequestAPI.Post<ZaloResponseCreateOrder>(url, new FormUrlEncodedContent(model));
                                if (response != null)
                                {
                                    if (response.returncode == 1)
                                    {
                                        await TimerCheckOrderZaloAsync(transaction, key1, AppId);
                                        if (isWeb)
                                        {
                                            //render QR code
                                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                            QRCodeData qrCodeData = qrGenerator.CreateQrCode(response.orderurl, QRCodeGenerator.ECCLevel.Q);
                                            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                                            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);

                                            return Json(new
                                            {
                                                Status = 200,
                                                Data = "data:image/jpeg;base64," + Crypto.SaltStr(qrCodeAsPngByteArr)
                                            });
                                        }
                                        return Json(new
                                        {
                                            Status = 200,
                                            Data = response
                                        });
                                    }
                                }

                                return BadRequest(new
                                {
                                    Status = 400,
                                    Message = "Tạo giao dịch thất bại"
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            FileSystem.WriteExceptionFile(ex.ToString(), "ZaloPay_user_id_" + IdUser.ToString());
                        }
                    }
                }
            }
            return BadRequest(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
        private async Task TimerCheckOrderZaloAsync(HistoryTransaction transaction, string key1, string appid)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            DoAnTotNghiepContext inputContext = new DoAnTotNghiepContext(this._context.GetConfig());
            string host = this.GetWebsitePath();
            Uri uri = RequestAPI.CheckOrderZaloRequest();
            Dictionary<string, string> model = new ZaloViewModel().CheckOrder(appid, key1, transaction);
            CheckTransactionBackground Object = new CheckTransactionBackground(
                                            uri,
                                            new FormUrlEncodedContent(model),
                                            transaction.CreatedDate.AddDays(1),
                                            new ChatHub(this._signalContext),
                                            this._configuration,
                                            transaction);
            TimedHostedService timer = new TimedHostedService(
                                            inputContext,
                                            host,
                                            TargetFunction.ExecuteCheckTransaction,
                                            token,
                                            30,
                                            TimeSpan.Zero,
                                            Object);
            //kiểm tra thử minus bao lâu nhớ sửa
            await timer.StartAsync(token);
        }

        //get CallBack
        [HttpPost("/Transaction/CallBack")]
        public async Task<IActionResult> CallBackZaloAsync([FromBody] dynamic cbdata)
        {
            ChatHub chatHub = new ChatHub(this._signalContext);
            chatHub.SendPaymentAll(TargetSignalR.Payment(), Convert.ToString(cbdata["data"]) + " - " + Convert.ToString(cbdata["mac"])).Wait(TimeSpan.FromMinutes(5));
            string? key2 = this._configuration.GetConnectionString(ZaloPay.ZaloKey2);
            var result = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(key2))
            {
                var DBtransaction = this._context.Database.BeginTransaction();

                try
                {
                    var dataStr = Convert.ToString(cbdata["data"]);
                    var reqMac = Convert.ToString(cbdata["mac"]);
                    var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key2, dataStr);

                    if (!reqMac.Equals(mac))
                    {
                        result["returncode"] = -1;
                        result["returnmessage"] = "mac not equal";
                    }
                    else
                    {
                        var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                        HistoryTransaction? historyTransaction = JsonConvert.DeserializeObject<HistoryTransaction>(dataJson["item"]);
                        if(historyTransaction != null)
                        {
                            HistoryTransaction? model = this._transactionService.GetById(historyTransaction.Id);
                            if(model != null)
                            {
                                if(model.Status != (int)StatusTransaction.VALID)
                                {
                                    model.Status = (int)StatusTransaction.VALID;
                                    this._transactionService.Update(model);

                                    model.IncludeAll(this._context);
                                    if (model.Users != null)
                                    {
                                        model.Users.Point += model.Amount;
                                        this._userService.UpdateUser(model.Users);
                                    }

                                    //signalR => notification
                                    Notification notification = new Notification()
                                    {
                                        Content = "Bạn đã nạp tiền thành công",
                                        CreatedDate = DateTime.Now,
                                        Title = "Nạp tích lũy",
                                        Type = NotificationType.PAYMENT,
                                        IdType = model.Id,
                                        IdUser = model.IdUser,
                                        IsSeen = false,
                                        ImageUrl = NotificationImage.Coin
                                    };
                                    this._notificationService.SaveNotification(notification);
                                    await chatHub.SendNotification(Crypto.EncodeKey(model.IdUser.ToString(), Crypto.Salt(this._configuration)),
                                        TargetSignalR.Notification(), new NotificationViewModel(notification, this.GetWebsitePath()));
                                }
                                
                                result["returncode"] = 1;
                                result["returnmessage"] = "success";
                            }
                            else
                            {
                                result["returncode"] = 0;
                                result["returnmessage"] = "item is not found";
                            }
                        }
                        else
                        {
                            result["returncode"] = 0;
                            result["returnmessage"] = "item is not found";
                        }
                    }
                    DBtransaction.Commit();
                }
                catch (Exception ex)
                {
                    result["returncode"] = 0;
                    result["returnmessage"] = ex.Message;
                    DBtransaction.Rollback();
                }
            }
            else
            {
                result["returncode"] = 0; // ZaloPay server sẽ callback lại (tối đa 3 lần)
                result["returnmessage"] = "Error";
            }
            return Ok(result);
        }

        [HttpGet("/api/Payment/All")]
        public IActionResult ApiGetAll(int year = 2023)
        {
            return this.GetTransaction(null);
        }
        [HttpGet("/api/Payment/Valid")]
        public IActionResult ApiGetValid(int year = 2023)
        {
            return this.GetTransaction((int)StatusTransaction.VALID);
        }
        [HttpGet("/api/Payment/Used")]
        public IActionResult ApiGetUsed(int year = 2023)
        {
            return this.GetTransaction((int)StatusTransaction.USED);
        }
        private IActionResult GetTransaction(int? Status, int year = 2023)
        {
            return Json(new
            {
                Status = 200,
                Data = this._transactionService.GetTransaction((int)StatusTransaction.VALID, this.GetIdUser(), year)
            }) ;
        }

        [HttpGet("/Statistics/Payment/All")]
        public IActionResult GetAll(int year = 2023)
        {
            return this.GetTransaction(null, year);
        }
        [HttpGet("/Statistics/Payment/Valid")]
        public IActionResult GetValid(int year = 2023)
        {
            return this.GetTransaction((int)StatusTransaction.VALID, year);
        }
        [HttpGet("/Statistics/Payment/Used")]
        public IActionResult GetUsed(int year = 2023)
        {
            return this.GetTransaction((int)StatusTransaction.USED, year);
        }

        [HttpGet("/Payment/Detail")]
        public IActionResult GetById(int Id)
        {
            return PartialView("~/Views/HistoryTransaction/_item.cshtml", this._transactionService.GetById(Id));
        }
    }
}