using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Authorization;
using DoAnTotNghiep.TrainModels;
using NuGet.Protocol;

namespace DoAnTotNghiep.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, 
                                DoAnTotNghiepContext context, 
                                IConfiguration _configuration,
                                IHostEnvironment environment) : base(environment)
        {
            _logger = logger;
            this._context = context;
            this._configuration = _configuration;
        }

        public IActionResult Index()
        {
            List<DetailHouseViewModel> houses = this.GetPopularHouse(12);
            List<PopularCityViewModel> cities = this.GetPopularCity(4);

            int IdUser = this.GetIdUser();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            /*,
                NewSuggest = this.GetCircleRequest(salt, host, IdUser),*/
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, salt);
            }
            return View(new HomeViewModel()
            {
                PopularCities = cities,
                PopularHouses = houses,
                NumberCities = this.NumberCity(),
                NumberHouses = this.NumberHouse(),
                NewRequests = this.GetRequest(salt, host, IdUser)
            });
        }

        public IActionResult TrainModel(string path)
        {
            string location = @"" + path;
            List<ViewModelTrained> list = new List<ViewModelTrained>();

            for(var value = 1; value <= 1; value++)
            {
                list.Add(new PredictHouse().ReTrain(location, value, 0.2f ));
            }

            return Json(list);
        }

        private List<DetailHouseViewModel> GetPopularHouse(int number = 10)
        {
            var listHouse = this.GetContextHouses(number);
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            List<DetailHouseViewModel> res = new List<DetailHouseViewModel>();
            foreach (var item in listHouse)
            {
                //this._context.Entry(item).Collection(m => m.RulesInHouses).Query().Load();
                //this._context.Entry(item).Collection(m => m.UtilitiesInHouses).Query().Load();
                //this._context.Entry(item).Reference(m => m.Users).Query().Load();

                item.IncludeLocation(this._context);

                //this._context.Entry(item).Reference(m => m.Citys).Query().Load();
                //this._context.Entry(item).Reference(m => m.Districts).Query().Load();
                //this._context.Entry(item).Reference(m => m.Wards).Query().Load();
                //this._context.Entry(item).Collection(m => m.Requests).Query().Load();
                //this._context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();
                //this._context.Entry(item).Collection(m => m.FeedBacks).Query().Load();

                DetailHouseViewModel model = new DetailHouseViewModel(item, salt, item.Users, host);
                if (item.FileOfHouses != null)
                {
                    foreach (var f in item.FileOfHouses)
                    {
                        this._context.Entry(f).Reference(m => m.Files).Load();
                        if (f.Files != null)
                        {
                            model.Images.Add(new ImageBase(f.Files, host));
                            break;
                        }
                    }
                }
                res.Add(model);
            }
            return res;
        }
        private List<House> GetContextHouses(int number = 10)
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            return this._context.Houses
                                .Include(m => m.Requests)
                                .Take(number)
                                .OrderByDescending(m => m.Rating)
                                .Where(m => m.Status == (int)StatusHouse.VALID)
                                .ToList();
        }
        private List<PopularCityViewModel> GetPopularCity(int number = 10)
        {
            string host = this.GetWebsitePath();
            var cities = from c in this._context.Cities
                         join h in this._context.Houses on c.Id equals h.IdCity
                         select c;
            List<City> cities1 = (cities != null ? cities.ToList() : new List<City>());
            List<PopularCityViewModel> cityList = cities1
                                                .OrderByDescending(m => m.Count)
                                                .Take(number)
                                                .Where(m => m.houses != null && m.houses.Any())
                                                .Select(m => new PopularCityViewModel()
                                                {
                                                    Name = m.Name,
                                                    Id = m.Id,
                                                    ImageUrl = host + "/Image/house-demo.png",
                                                    IsDeleted = false,
                                                    Location = new PointViewModel()
                                                    {
                                                        Lat = m.Lat,
                                                        Lng = m.Lng
                                                    }

                                                }
                                                )
                                                .ToList();
            return cityList;
        }
        private int NumberCity()
        {
            var cities = this._context.Cities.Include(m => m.houses)
                                        .Where(m => m.houses != null 
                                            && m.houses.Any(h => h.Status == (int) StatusHouse.VALID));

            return cities == null? 0: cities.ToList().Count();
        }
        private int NumberHouse()
        {
            return this._context.Houses
                                .Where(m => m.Status == (int)StatusHouse.VALID)
                                .ToList().Count();
        }
        private List<DetailRequest> GetRequest(byte[] salt, string host, int IdUser = 0)
        {
            List<DetailRequest> model = new List<DetailRequest>();
            if (IdUser != 0)
            {
                //LẤY NHỮNG REQUEST CHỜ + ACCEPT -> CHECK_OUT
                var rq = this._context.Requests
                                            .Include(m => m.Houses)
                                            .Where(m => (m.Status == (int)StatusRequest.ACCEPT
                                                            || m.Status == (int)StatusRequest.CHECK_IN
                                                            || m.Status == (int)StatusRequest.WAIT_FOR_SWAP)
                                                        && (m.IdUser == IdUser || m.Houses != null && m.Houses.IdUser == IdUser))
                                            .ToList();

                if (rq != null)
                {
                    var nrq = rq.OrderBy(m => m.StartDate).ToList();
                    foreach(var item in nrq)
                    {
                        DetailRequest? request = this.CreateDetailRequest(item, null, salt, host);
                        if (request != null) model.Add(request);
                    }
                }
            }
            return model;
        }
        private DetailRequest? CreateDetailRequest(Request item, int? Status, byte[] salt, string host)
        {
            int IdUser = this.GetIdUser();
            this._context.Entry(item).Reference(m => m.Houses).Query().Load();
            this._context.Entry(item).Collection(m => m.FeedBacks).Query().Where(m => m.IdUser == IdUser).Load();
            this._context.Entry(item).Collection(m => m.CheckOuts).Query().Where(m => m.IdUser == IdUser).Load();
            this._context.Entry(item).Collection(m => m.CheckIns).Query().Where(m => m.IdUser == IdUser).Load();
            item.CheckStatus(item);
            if (Status != null)
            {
                if (item.Status != Status)
                {
                    return null;
                }
            }
            if (item.Houses != null)
            {
                this._context.Entry(item.Houses).Reference(m => m.Users).Query().Load();
                this._context.Entry(item.Houses).Collection(m => m.FileOfHouses).Query().Load();
                if (item.Houses.Users != null)
                {
                    DetailHouseViewModel house = this.CreateDetailsHouse(item.Houses, salt, host);
                    DetailHouseViewModel? swapHouse = null;

                    this._context.Entry(item).Reference(m => m.Users).Query().Load();
                    if (item.IdSwapHouse != null)
                    {
                        this._context.Entry(item).Reference(m => m.SwapHouses).Query().Load();
                        if (item.SwapHouses != null)
                        {
                            this._context.Entry(item.SwapHouses).Reference(m => m.Users).Query().Load();
                            this._context.Entry(item.SwapHouses).Collection(m => m.FileOfHouses).Query().Load();
                            swapHouse = this.CreateDetailsHouse(item.SwapHouses, salt, host);
                        }
                    }
                    User? inputUser = null;
                    if (item.IdUser != IdUser)
                    {
                        inputUser = item.Houses.Users;//false
                    }
                    else
                    {
                        inputUser = item.Users;//true
                    }
                    DetailRequestViewModel request = new DetailRequestViewModel(item, inputUser, salt, host);
                    return new DetailRequest()
                    {
                        Request = request,
                        SwapHouse = swapHouse,
                        House = house,
                        UserRating = null,
                        MyRating = null
                    };
                }
            }
            return null;
        }
        private DetailHouseViewModel CreateDetailsHouse(House house, byte[] salt, string host)
        {
            if (house.Users != null)
            {
                this._context.Entry(house.Users).Collection(m => m.Houses).Query().Where(m => m.Status == (int)StatusHouse.VALID).Load();
            }
            house.IncludeLocation(this._context);
            DetailHouseViewModel model = new DetailHouseViewModel(house, salt, house.Users, host);
            if (house.FileOfHouses != null)
            {
                foreach (var f in house.FileOfHouses)
                {
                    this._context.Entry(f).Reference(m => m.Files).Load();
                    if (f.Files != null)
                    {
                        model.Images.Add(new ImageBase(f.Files, host));
                        break;
                    }
                }
            }
            return model;
        }

        private List<CircleRequestViewModel> GetCircleRequest(byte[] salt, string host, int IdUser = 0)
        {
            List<CircleRequestViewModel> model = new List<CircleRequestViewModel>();
            if (IdUser != 0)
            {
                DateTime now = DateTime.Now;
                var circle = this._context.CircleExchangeHouseOfUsers
                                        .Include(m => m.CircleExchangeHouse)
                                        .Where(m => m.IdUser == IdUser
                                                        && m.CircleExchangeHouse != null
                                                        && m.CircleExchangeHouse.Status != (int)StatusWaitingRequest.DISABLE
                                                        && DateTime.Compare(m.CircleExchangeHouse.StartDate, now) <= 0
                                                        && DateTime.Compare(m.CircleExchangeHouse.EndDate, now) >= 0)
                                        .Select(m => m.CircleExchangeHouse)
                                        .ToList();
                foreach (var item in circle)
                {
                    if (item != null)
                    {
                        CircleRequestViewModel? circleRequest = this.CreateDetailCircleRequest(item, host, salt, IdUser);
                        if (circleRequest != null) model.Add(circleRequest);
                    }
                }
            }
            return model;
        }
        private CircleRequestViewModel? CreateDetailCircleRequest(CircleExchangeHouse item, string host, byte[] salt, int IdUser)
        {
            this._context.Entry(item).Collection(m => m.RequestInCircles).Query().Load();
            this._context.Entry(item).Collection(m => m.FeedBacks).Load();
            if (item.RequestInCircles != null)
            {
                var rq = from rqc in item.RequestInCircles
                         join crq in this._context.WaitingRequests on rqc.IdWaitingRequest equals crq.Id
                         select crq.Node(rqc, crq);
                if (rq != null)
                {
                    rq = rq.ToList();
                    foreach (var itemRQ in rq)
                    {
                        this._context.Entry(itemRQ).Reference(m => m.Houses).Load();
                    }
                    WaitingRequest? myNode = rq.Where(m => m.IdUser == IdUser).FirstOrDefault();
                    if (myNode != null)
                    {
                        if (myNode.Houses != null)
                        {
                            myNode.Houses.IncludeLocation(this._context);
                            var inputMyNode = this.CircleRequestDetail(myNode, salt, host);
                            WaitingRequest? prevNode = rq.Where(m => m.IdUser != IdUser && m.IdCity == myNode.Houses.IdCity.Value).FirstOrDefault();
                            if (prevNode != null && inputMyNode != null)
                            {
                                prevNode.Houses.IncludeLocation(this._context);
                                var inputPrevNode = this.CircleRequestDetail(prevNode, salt, host);
                                WaitingRequest? nextNode = rq.Where(m => m.IdUser != IdUser && myNode.IdCity == m.Houses.IdCity.Value).FirstOrDefault();
                                if (nextNode != null && inputPrevNode != null)
                                {
                                    nextNode.Houses.IncludeLocation(this._context);
                                    var inputNextNode = this.CircleRequestDetail(nextNode, salt, host);

                                    if (inputNextNode != null)
                                    {
                                        return new CircleRequestViewModel(inputPrevNode, inputMyNode, inputNextNode, item, IdUser);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        private CircleRequestDetail? CircleRequestDetail(WaitingRequest request, byte[] salt, string host)
        {
            //lấy nhà
            if (request.Houses != null)
            {
                //lấy imagehouse
                this._context.Entry(request.Houses).Collection(m => m.FileOfHouses).Query().Load();
                if (request.Houses.FileOfHouses != null)
                {
                    var fImage = request.Houses.FileOfHouses.FirstOrDefault();
                    if (fImage != null)
                    {
                        this._context.Entry(fImage).Reference(m => m.Files).Load();
                        if (fImage != null)
                        {
                            ImageBase HouseImage = new ImageBase(fImage.Files, host);

                            //lấy người dùng
                            this._context.Entry(request).Reference(m => m.Users).Load();
                            if (request.Users != null)
                            {
                                this._context.Entry(request.Users).Reference(m => m.Files).Load();
                                UserInfo user = new UserInfo(request.Users, salt, host);
                                var numberSwap = from u in this._context.Users
                                                 join h in this._context.Houses on u.Id equals h.IdUser
                                                 join rq in this._context.Requests on h.Id equals rq.IdHouse
                                                 where u.Id == request.Users.Id && rq.Status >= (int)StatusRequest.CHECK_IN
                                                 select rq;
                                user.NumberSwap = (numberSwap == null ? 0 : numberSwap.ToList().Count());


                                this._context.Entry(request.Houses).Collection(m => m.Requests).Query().Where(m => m.Status == (int)StatusRequest.WAIT_FOR_SWAP).Load();
                                this._context.Entry(request.Houses).Collection(m => m.FeedBacks).Query().Load();

                                //this._context.Entry(request).Reference(m => m.Houses).Load();
                                return new CircleRequestDetail(new DetailHouseViewModel(request.Houses, salt, null, null), request, user, HouseImage);
                            }
                        }
                    }
                }
            }
            return null;
        }

        private IActionResult NewUser()
        {
            byte[] salt1 = Crypto.Salt();
            User user1 = new User()
            {
                FirstName = "Trang Phúc",
                LastName = "Bảo",
                Gender = true,
                BirthDay = DateTime.Now,
                Email = "bao.trang@gmail.com",
                Password = Crypto.HashPass("Vinova123", salt1),
                Salt = Crypto.SaltStr(salt1),
                PhoneNumber = "0973409127",
                Point = 10000,
                BonusPoint = 10000,
                IdFile = null,
                UserRating = 0,
                Role = Role.MemberCode
            };


            byte[] salt2 = Crypto.Salt();
            User user2 = new User()
            {
                FirstName = "Phạm Đăng",
                LastName = " Khoa",
                Gender = true,
                BirthDay = DateTime.Now,
                Email = "khoa.dang@gmail.com",
                Password = Crypto.HashPass("Vinova123", salt2),
                Salt = Crypto.SaltStr(salt1),
                PhoneNumber = "0973409127",
                Point = 10000,
                BonusPoint = 10000,
                IdFile = null,
                UserRating = 0,
                Role = Role.MemberCode
            };

            this._context.Users.Add(user1);
            this._context.Users.Add(user2);
            this._context.SaveChanges();
            return Ok();
        }
        public IActionResult TestCreateWaitingRequest()
        {
            //test Waiting Request + Waiting Request
            var ListUserId = this._context.Users.Select(m => m.Id).ToList();
            //test vòng 3
            //không có
            List<WaitingRequest> waitingRequests = new List<WaitingRequest>()
            {
                new WaitingRequest().CreateModel(new CreateWaitingRequest(1, 11, DateTime.Now, DateTime.Now.AddDays(1)), 21),//3->1
                new WaitingRequest().CreateModel(new CreateWaitingRequest(55, 11, DateTime.Now, DateTime.Now.AddDays(1)), 21),//3->55
                new WaitingRequest().CreateModel(new CreateWaitingRequest(56, 11, DateTime.Now, DateTime.Now.AddDays(1)), 21),//3->56
                new WaitingRequest().CreateModel(new CreateWaitingRequest(4, 11, DateTime.Now, DateTime.Now.AddDays(1)), 21),//3->4
                new WaitingRequest().CreateModel(new CreateWaitingRequest(3, 13, DateTime.Now, DateTime.Now.AddDays(2)), 23),//55->3
                new WaitingRequest().CreateModel(new CreateWaitingRequest(62, 12, DateTime.Now, DateTime.Now.AddDays(2)), 22),//56->62
                new WaitingRequest().CreateModel(new CreateWaitingRequest(62, 9, DateTime.Now, DateTime.Now.AddDays(2)), 17),//4->62
                new WaitingRequest().CreateModel(new CreateWaitingRequest(62, 11, DateTime.Now, DateTime.Now.AddDays(2)), 20),//1B->62
                new WaitingRequest().CreateModel(new CreateWaitingRequest(1, 10, DateTime.Now, DateTime.Now.AddDays(2)), 19)//62->1B
            };//hiện 

            this._context.WaitingRequests.AddRange(waitingRequests);
            this._context.SaveChanges();

            /*
             * List<WaitingRequest> waitingRequests = new List<WaitingRequest>()
            {
                new WaitingRequest().CreateModel(new CreateWaitingRequest(1, 9, null, null), 17),
                new WaitingRequest().CreateModel(new CreateWaitingRequest(3, 10, null, null), 19),
                new WaitingRequest().CreateModel(new CreateWaitingRequest(1, 10, null, null), 19),
                new WaitingRequest().CreateModel(new CreateWaitingRequest(4, 10, null, null), 19),
                new WaitingRequest().CreateModel(new CreateWaitingRequest(55, 11, null, null), 21),
                new WaitingRequest().CreateModel(new CreateWaitingRequest(56, 12, null, null), 22),
                new WaitingRequest().CreateModel(new CreateWaitingRequest(62, 11, null, null), 21)
            };
             */

            //tạo random time
            //test Request + Request
            //test Waiting Request + Request
            this.CreateCircleSwap(5);
            return Ok();
        }
        public IActionResult AddNode()
        {
            this._context.WaitingRequests.Add(new WaitingRequest().CreateModel(new CreateWaitingRequest(55, 10, DateTime.Now, DateTime.Now.AddDays(2)), 18));
            this._context.SaveChanges();
            this.CreateCircleSwap(5);
            return Ok();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //chạy tốt k lỗi => kiểm tra chính xác thôi
        private List<ObjectCircleAdd> CreateCircleSwap(int maxSize = 5)
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
            while (!(requests.Count > 0 || minSize > maxSize || minSize > requestSearch.Count()))
            {
                //list đã được order theo ToIdCity => duyệt list 1 lần
                for (int index = 0; index < requestSearch.Count(); index++)
                {
                    List<WaitingRequestForSearch> listBase = requestSearch.GetRange(index + 1, requestSearch.Count() - (index + 1));
                    List<WaitingRequestForSearch> head = new List<WaitingRequestForSearch>() { requestSearch.ElementAt(index) };
                    Recursive(listBase, minSize, head, requests);
                }
                minSize++;
            }

            /*try
            var DBtransaction = this._context.Database.BeginTransaction();

            {
                List<ObjectCircleAdd> objectCircleAdds = new List<ObjectCircleAdd>();
                foreach (var item in requests)
                {
                    if (item.Count() >= 3)
                    {
                        WaitingRequestForSearch Last = item.Last();
                        if(Last.StartDate != null && Last.EndDate != null)
                        {
                            var listObj = item.Select(m => m.Request).ToList();
                            //danh sách trong 1 vòng
                            var newWaitingNode = listObj.Where(m => m.Id == 0).ToList();
                            this._context.WaitingRequests.AddRange(newWaitingNode);//thêm request mới => thêm do từ request -> và đã incircle

                            var oldWaitingNode = listObj.Where(m => m.Id != 0).ToList();
                            foreach (var ud in oldWaitingNode) ud.Status = (int)StatusWaitingRequest.IN_CIRCLE;
                            this._context.WaitingRequests.UpdateRange(oldWaitingNode);//update Status
                            this._context.SaveChanges();
                            CircleExchangeHouse circleExchange = new CircleExchangeHouse() { 
                                Status = (int)StatusWaitingRequest.INIT, 
                                StartDate = Last.StartDate.Value, 
                                EndDate = Last.EndDate.Value 
                            };
                            this._context.CircleExchangeHouses.AddRange(circleExchange);
                            this._context.SaveChanges();
                            objectCircleAdds.Add(new ObjectCircleAdd(circleExchange, listObj));
                        }
                    }
                }

                foreach (var item in objectCircleAdds)
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
                return objectCircleAdds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                DBtransaction.Rollback();
            }*/

            List<ObjectCircleAdd> objectCircleAdds = new List<ObjectCircleAdd>();
            foreach (var item in requests)
            {
                if (item.Count() >= 3)
                {
                    WaitingRequestForSearch Last = item.Last();
                    if (Last.StartDate != null && Last.EndDate != null)
                    {
                        var listObj = item.Select(m => m.Request).ToList();
                        //danh sách trong 1 vòng
                        var newWaitingNode = listObj.Where(m => m.Id == 0).ToList();
                        this._context.WaitingRequests.AddRange(newWaitingNode);//thêm request mới => thêm do từ request -> và đã incircle

                        var oldWaitingNode = listObj.Where(m => m.Id != 0).ToList();
                        foreach (var ud in oldWaitingNode) ud.Status = (int)StatusWaitingRequest.IN_CIRCLE;
                        this._context.WaitingRequests.UpdateRange(oldWaitingNode);//update Status
                        this._context.SaveChanges();
                        CircleExchangeHouse circleExchange = new CircleExchangeHouse()
                        {
                            Status = (int)StatusWaitingRequest.INIT,
                            StartDate = Last.StartDate.Value,
                            EndDate = Last.EndDate.Value
                        };
                        this._context.CircleExchangeHouses.AddRange(circleExchange);
                        this._context.SaveChanges();
                        objectCircleAdds.Add(new ObjectCircleAdd(circleExchange, listObj));
                    }
                }
            }

            foreach (var item in objectCircleAdds)
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

            Email sender = new Email("hieu.phamgc@gmail.com", "epnmbhwjbxaruean");
            string body = "check: ok\n";
            body += objectCircleAdds.Count().ToString();
            sender.SendMail("hieu.phamgc@gmail.com", "check", body, null, string.Empty);

            return objectCircleAdds;
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

    }
}