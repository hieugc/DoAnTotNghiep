using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.ViewModels;
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using DoAnTotNghiep.Enum;
using NuGet.Protocol;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using NuGet.Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
using System.Composition;
using DoAnTotNghiep.TrainModels;
using DoAnTotNghiep.Service;
using System.Globalization;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class AdminController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IFeedBackService _feedBackService;
        private readonly IHouseService _houseService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly IRequestService _requestService;
        private readonly IRuleService _ruleService;
        private readonly IUtilitiesService _utilitiesService;
        private readonly ICircleRequestService _circleRequestService;
        private readonly INotificationService _notificationService;

        public AdminController(DoAnTotNghiepContext context, 
                                IConfiguration configuration,
                                IHostEnvironment environment,
                                IFeedBackService feedBackService,
                                IRequestService requestService,
                                IFileService fileService,
                                IHouseService houseService,
                                IUserService userService,
                                IRuleService ruleService,
                                IUtilitiesService utilitiesService,
                                ICircleRequestService circleRequestService,
                                INotificationService notificationService) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _feedBackService = feedBackService;
            _requestService = requestService;
            _fileService = fileService;
            _houseService = houseService;
            _userService = userService;
            _ruleService = ruleService;
            _utilitiesService = utilitiesService;
            _circleRequestService = circleRequestService;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            ViewData["index"] = 0;
            return View();
        }
        /*
        public IActionResult Utilities()
        {
            ViewData["index"] = 2;
            return View(this._utilitiesService.All());
        }
        public IActionResult Rules()
        {
            ViewData["index"] = 3;
            return View(this._ruleService.All());
        }*/
        public IActionResult Users(Pagination pagination)
        {
            ViewData["index"] = 1;
            var lUser = this._userService.All();
            int skip = (pagination.Page - 1 <= 0 ? 0 : pagination.Page - 1);
            pagination.Total = (int) Math.Ceiling((double) (lUser.Count() / pagination.Limit));
            var model = lUser.Skip(skip * pagination.Limit).Take(pagination.Limit).ToList();
            foreach (var item in model) item.InCludeAll(this._context);
            return View(model);
        }

        private int WeeksInYear(DateTime date)
        {
            GregorianCalendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);
            return cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday); 
        }

        [HttpGet("/Admin/House")]
        public int AllHouse(int option = 0)
        {
            var houses = this._houseService.All();
            int number = 0;
            switch (option)
            {
                case 0://tuần
                    int week = WeeksInYear(DateTime.Now);
                    number = houses.Where(m => week == this.WeeksInYear(m.CreatedDate)).Count();
                    break;
                case 1://tháng
                    int month = DateTime.Now.Month;
                    number = houses.Where(m => m.CreatedDate.Month == month).Count();
                    break;
                case 2://năm
                    int year = DateTime.Now.Year;
                    number = houses.Where(m => m.CreatedDate.Year == year).Count();
                    break;
            }
            return number;
        }

        [HttpGet("/Admin/FeedBack")]
        public int AllFeedBack(int option = 0)
        {
            var houses = this._feedBackService.All();
            int number = 0;
            switch (option)
            {
                case 0://tuần
                    int week = WeeksInYear(DateTime.Now);
                    number = houses.Where(m => week == this.WeeksInYear(m.CreatedDate)).Count();
                    break;
                case 1://tháng
                    int month = DateTime.Now.Month;
                    number = houses.Where(m => m.CreatedDate.Month == month).Count();
                    break;
                case 2://năm
                    int year = DateTime.Now.Year;
                    number = houses.Where(m => m.CreatedDate.Year == year).Count();
                    break;
            }
            return number;
        }

        [HttpGet("/Admin/User")]
        public GDP AllUser(int option = 0)
        {
            var houses = this._userService.All().Where(m => m.Role != Role.AdminCode).ToList();
            double pourCent = 0.0;
            switch (option)
            {
                case 0://tuần
                    int week = WeeksInYear(DateTime.Now);
                    pourCent = houses.Where(m => week == this.WeeksInYear(m.CreatedDate)).Count();
                    break;
                case 1://tháng
                    int month = DateTime.Now.Month;
                    pourCent = houses.Where(m => m.CreatedDate.Month == month).Count();
                    break;
                case 2://năm
                    int year = DateTime.Now.Year;
                    pourCent = houses.Where(m => m.CreatedDate.Year == year).Count();
                    break;
            }
            return new GDP(houses.Count(), pourCent);
        }

        [HttpGet("/Admin/Report")]
        public GDP AllReport(int option = 0)
        {
            var houses = this._context.UserReports.ToList();
            double pourCent = 0.0;
            switch (option)
            {
                case 0://tuần
                    int week = WeeksInYear(DateTime.Now);
                    pourCent = houses.Where(m => week == this.WeeksInYear(m.CreatedDate)).Count();
                    break;
                case 1://tháng
                    int month = DateTime.Now.Month;
                    pourCent = houses.Where(m => m.CreatedDate.Month == month).Count();
                    break;
                case 2://năm
                    int year = DateTime.Now.Year;
                    pourCent = houses.Where(m => m.CreatedDate.Year == year).Count();
                    break;
            }
            return new GDP(houses.Count(), pourCent);
        }

        [HttpGet("/Admin/Request")]
        public RequestAdmin AllRequest(int option = 0)
        {
            var houses = this._requestService.All();
            int total = 0;
            int accept = 0;
            switch (option)
            {
                case 0://tuần
                    int week = WeeksInYear(DateTime.Now);
                    var w = houses.Where(m => week == this.WeeksInYear(m.CreatedDate)).ToList();
                    total = w.Count();
                    accept = w.Where(m => m.Status != (int)StatusRequest.WAIT_FOR_SWAP && m.Status != (int)StatusRequest.DISABLE && m.Status != (int)StatusRequest.REJECT).Count();
                    break;
                case 1://tháng
                    int month = DateTime.Now.Month;
                    var m = houses.Where(m => month == this.WeeksInYear(m.CreatedDate)).ToList();
                    total = m.Count();
                    accept = m.Where(m => m.Status != (int)StatusRequest.WAIT_FOR_SWAP && m.Status != (int)StatusRequest.DISABLE && m.Status != (int)StatusRequest.REJECT).Count();
                    break;
                case 2://năm
                    int year = DateTime.Now.Year;
                    var y = houses.Where(m => year == this.WeeksInYear(m.CreatedDate)).ToList();
                    total = y.Count();
                    accept = y.Where(m => m.Status != (int)StatusRequest.WAIT_FOR_SWAP && m.Status != (int)StatusRequest.DISABLE && m.Status != (int)StatusRequest.REJECT).Count();
                    break;
            }
            return new RequestAdmin(total, accept, (total - accept));
        }

        [HttpGet("/Admin/Transaction")]
        public JsonResult AllTransaction(int option = 0)
        {
            DateTime now = DateTime.Now;
            var houses = this._context.HistoryTransactions
                                        .Where(m => m.Status == (int)StatusTransaction.VALID && m.CreatedDate.Year == now.Year)
                                        .OrderBy(m => m.CreatedDate)
                                        .Select(m => new {
                                            Value = m.Amount,
                                            Month = m.CreatedDate.Month
                                        })
                                        .ToList();
            return Json(houses);
        }

        public IActionResult Model()
        {
            return View("~/Views/Admin/Model.cshtml", new PredictHouse().GetSquare());
        }
        [HttpGet("/UpdateTransaction")]
        public IActionResult UpdateTransaction()
        {
            var transaction = this._context.HistoryTransactions.Where(m => m.Status == (int)StatusTransaction.VALID).ToList();
            foreach (var item in transaction) item.Content = "Bạn đã nạp "
                                    + PriceFormat.VNDFormat((item.Amount * 1000))
                                    + " vào hệ thống tích lũy điểm lúc "
                                    + item.CreatedDate.ToString("hh:mm:ss dd/MM/yyyy");

            this._context.HistoryTransactions.UpdateRange(transaction);
            this._context.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }
        [HttpGet("/UpdateCircle")]
        public IActionResult UpdateCircle()
        {
            var transaction = this._context.RequestsInCircleExchangeHouses.Where(m => m.Status == (int)StatusWaitingRequest.INIT).ToList();
            foreach (var item in transaction) item.Status = (int)StatusWaitingRequest.IN_CIRCLE;

            this._context.RequestsInCircleExchangeHouses.UpdateRange(transaction);
            this._context.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }
    }
}
