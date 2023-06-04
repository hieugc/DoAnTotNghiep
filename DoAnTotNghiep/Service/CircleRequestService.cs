using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Job;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static QRCoder.PayloadGenerator;

namespace DoAnTotNghiep.Service
{
    public class CircleRequestService : ICircleRequestService
    {
        private DoAnTotNghiepContext _context;
        public CircleRequestService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }

        public List<WaitingRequestForSearch> GetInitWaitingRequest()
        {
            var waitingRq = this._context.WaitingRequests
                                        .Include(m => m.Houses)
                                        .Where(m => (m.Status == (int)StatusWaitingRequest.INIT)
                                                        && m.Houses != null
                                                        && m.Houses.IdCity != null)
                                        .Select(m => new WaitingRequestForSearch(m, m.IdCity, m.Houses.IdCity.Value, m.StartDate, m.EndDate))
                                        .ToList();

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

            return requestSearch;
        }
        public void Save(List<List<WaitingRequestForSearch>> requests)
        {
            var DBtransaction = this._context.Database.BeginTransaction();
            try
            {
                //chưa kiểm tra vòng tồn tại
                List<ObjectCircleAdd> objectCircleAdds = new List<ObjectCircleAdd>();
                foreach (var item in requests)
                {
                    if (item.Count() >= 3)
                    {
                        var listObj = item.Select(m => m.Request).ToList();
                        //danh sách trong 1 vòng
                        var newWaitingNode = listObj.Where(m => m.Id == 0).ToList();
                        this._context.WaitingRequests.AddRange(newWaitingNode);//thêm request mới => thêm do từ request -> và đã incircle

                        var oldWaitingNode = listObj.Where(m => m.Id != 0).ToList();
                        foreach (var ud in oldWaitingNode) ud.Status = (int)StatusWaitingRequest.IN_CIRCLE;
                        this._context.WaitingRequests.UpdateRange(oldWaitingNode);//update Status
                        this._context.SaveChanges();
                        CircleExchangeHouse circleExchange = new CircleExchangeHouse() { Status = (int)StatusWaitingRequest.INIT,StartDate = item.First().StartDate.Value,EndDate = item.First().EndDate.Value};
                        this._context.CircleExchangeHouses.AddRange(circleExchange);
                        this._context.SaveChanges();
                        objectCircleAdds.Add(new ObjectCircleAdd(circleExchange, listObj));
                    }
                }
                foreach (var item in objectCircleAdds)
                {
                    List<CircleExchangeHouseOfUser> newUser = new List<CircleExchangeHouseOfUser>();
                    List<RequestInCircleExchangeHouse> newLink = new List<RequestInCircleExchangeHouse>();
                    foreach (var u in item.WaitingRequests)
                    {
                        newUser.Add(new CircleExchangeHouseOfUser() { IdCircleExchangeHouse = item.CircleExchanges.Id, IdUser = u.IdUser });
                        newLink.Add(new RequestInCircleExchangeHouse() { IdCircleExchangeHouse = item.CircleExchanges.Id, IdWaitingRequest = u.Id , Status = (int)StatusWaitingRequest.IN_CIRCLE });
                    }
                    this._context.CircleExchangeHouseOfUsers.AddRange(newUser);
                    this._context.RequestsInCircleExchangeHouses.AddRange(newLink);
                }
                this._context.SaveChanges();
                DBtransaction.Commit();
                FileSystem.WriteExceptionFile(objectCircleAdds.Count().ToString(), "Info_Create_CircleRequest_" + DateTime.Now.ToString("hh_mm_ss_dd_MM_yyyy"));
            }
            catch (Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "Create_CircleRequest_" + DateTime.Now.ToString("hh_mm_ss_dd_MM_yyyy"));
                DBtransaction.Rollback();
            }
        }
        public void RemoveCircleSwap()
        {
            DateTime now = DateTime.Now.AddDays(-2);
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
            catch (Exception e)
            {
                FileSystem.WriteExceptionFile(e.ToString(), "Remove_CircleRequest_" + DateTime.Now.ToString("hh_mm_ss_dd_MM_yyyy"));
            }
        }
        public bool Remove(int idRequest)
        {
            var rq = this._context.CircleExchangeHouses.Where(m => m.Id == idRequest && (m.Status <= (int)StatusRequest.CHECK_IN)).FirstOrDefault();
            if (rq != null)
            {
                try
                {
                    rq.Status = (int)StatusWaitingRequest.DISABLE;
                    this._context.CircleExchangeHouses.Update(rq);
                    this._context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return false;
        }
        public void RemoveRequestOutDate()
        {
            DateTime now = DateTime.Now.AddDays(-2);
            var request = this._context.Requests
                                        .Where(m => m.Status == (int)StatusRequest.WAIT_FOR_SWAP && DateTime.Compare(m.StartDate, now) < 0)
                                        .ToList();
            foreach (var item in request)
            {
                item.Status = (int)StatusRequest.REJECT;
            }

            try
            {
                this._context.Requests.UpdateRange(request);
                this._context.SaveChanges();
            }
            catch (Exception e)
            {
                FileSystem.WriteExceptionFile(e.ToString(), "Remove_CircleOutDateRequest_" + DateTime.Now.ToString("hh_mm_ss_dd_MM_yyyy"));
            }
        }

        public void AddRange()
        {
            try
            {
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
                };

                this._context.WaitingRequests.AddRange(waitingRequests);
                this._context.SaveChanges();
                FileSystem.WriteExceptionFile("AddRange", "AddRange" + DateTime.Now.ToString("hh_mm_ss_dd_MM_yyyy"));
            }
            catch (Exception e)
            {
                FileSystem.WriteExceptionFile(e.ToString(), "AddRange" + DateTime.Now.ToString("hh_mm_ss_dd_MM_yyyy"));
            }
        }
        public void AddNode()
        {
            try
            {
                this._context.WaitingRequests.Add(new WaitingRequest().CreateModel(new CreateWaitingRequest(55, 10, DateTime.Now, DateTime.Now.AddDays(2)), 18));
                this._context.SaveChanges();
                FileSystem.WriteExceptionFile("Addnode", "Addnode" + DateTime.Now.ToString("hh_mm_ss_dd_MM_yyyy"));
            }
            catch (Exception e)
            {
                FileSystem.WriteExceptionFile(e.ToString(), "Addnode" + DateTime.Now.ToString("hh_mm_ss_dd_MM_yyyy"));
            }
        }
        public void Update(RequestInCircleExchangeHouse request)
        {
            this._context.RequestsInCircleExchangeHouses.Update(request);
            this._context.SaveChanges();
        }
        public void Update(CircleExchangeHouse request)
        {
            this._context.CircleExchangeHouses.Update(request);
            this._context.SaveChanges();
        }
        public List<List<WaitingRequestForSearch>> IsExist(List<List<WaitingRequestForSearch>> waitingRequestForSearches)
        {
            List<List<WaitingRequestForSearch>> model = new List<List<WaitingRequestForSearch>>();

            List<CircleExchangeHouse> allCircle = this._context.CircleExchangeHouses
                                                            .Include(m => m.RequestInCircles)
                                                            .Where(m => m.Status == (int)StatusWaitingRequest.INIT)
                                                            .ToList();
            foreach(var item in allCircle)
            {
                if(item.RequestInCircles != null)
                {
                    foreach(var r in item.RequestInCircles)
                    {
                        r.InCludeAll(this._context);
                    }
                }
            }

            foreach (var item in waitingRequestForSearches)
            {
                if(item.Count() > 0)
                {
                    var allList = allCircle.Where(m => m.RequestInCircles != null && m.RequestInCircles.Count() == item.Count() && m.StartDate == item.First().StartDate && m.EndDate == item.First().EndDate).ToList();
                    if(allList.Count() > 0)
                    {
                        List<int> IdUsers = item.OrderBy(m => m.IdUser).Select(m => m.IdUser).ToList();
                        List<int> IdCities = item.OrderBy(m => m.IdUser).Select(m => m.ToIdCity).ToList();
                        bool isExist = false;
                        foreach (var r in allList)
                        {
                            List<WaitingRequest> waitings = r.RequestInCircles.Select(m => m.WaitingRequests).ToList();
                            List<int> rIdUsers = waitings.OrderBy(m => m.IdUser).Select(m => m.IdUser).ToList();
                            bool isSame = true;
                            for(var index = 0; index < IdUsers.Count(); index++)
                            {
                                if(rIdUsers.ElementAt(index) != IdUsers.ElementAt(index))
                                {
                                    isSame = false;
                                    break;
                                }
                            }
                            if (isSame)
                            {
                                List<int> rIdCities = waitings.OrderBy(m => m.IdUser).Select(m => m.IdCity).ToList();
                                for (var index = 0; index < IdCities.Count(); index++)
                                {
                                    if (rIdCities.ElementAt(index) != IdCities.ElementAt(index))
                                    {
                                        isSame = false;
                                        break;
                                    }
                                }
                                if (isSame)
                                {
                                    isExist = true;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        if (!isExist) model.Add(item);
                    }
                    else
                    {
                        model.Add(item);
                    }
                }
            }


            return model;
        }
        public CircleExchangeHouse? GetById(int idRequest) => this._context.CircleExchangeHouses.Where(m => m.Id == idRequest && m.Status != (int)StatusWaitingRequest.DISABLE).FirstOrDefault();
        public RequestInCircleExchangeHouse? GetWaitingRequestById(int idRequest, int idUser)
        {
            return (from cr in this._context.CircleExchangeHouses
                    join cru in this._context.CircleExchangeHouseOfUsers on cr.Id equals cru.IdCircleExchangeHouse
                    join wr in this._context.RequestsInCircleExchangeHouses on cr.Id equals wr.IdCircleExchangeHouse
                    where cru.IdUser == idUser && cr.Id == idRequest && cr.Status != (int) StatusWaitingRequest.DISABLE
                    select wr).FirstOrDefault();
        }
        public CircleRequestDetail? CircleRequestDetail(WaitingRequest request, IUserService userService, byte[] salt, string host)
        {
            if (request.Houses != null)
            {
                request.Houses.IncludeAll(this._context);
                if (request.Houses.FileOfHouses != null)
                {
                    var fImage = request.Houses.FileOfHouses.FirstOrDefault();
                    if (fImage != null)
                    {
                        this._context.Entry(fImage).Reference(m => m.Files).Load();
                        if (fImage != null)
                        {
                            ImageBase HouseImage = new ImageBase(fImage.Files, host);
                            request.Users = userService.GetById(request.IdUser);
                            if (request.Users != null)
                            {
                                UserInfo.GetEntityRelated(request.Users, this._context);
                                UserInfo user = new UserInfo(request.Users, salt, host);
                                user.NumberSwap = userService.NumberSwap(request.Users.Id);
                                request.Houses.IncludeAll(this._context);
                                return new CircleRequestDetail(new DetailHouseViewModel(request.Houses, salt, null, null), request, user, HouseImage);
                            }
                        }
                    }
                }
            }
            return null;
        }
        public List<CircleExchangeHouse> GetByUser(int IdUser)
        {
            var model = from cru in this._context.CircleExchangeHouseOfUsers
                        join cr in this._context.CircleExchangeHouses on cru.IdCircleExchangeHouse equals cr.Id
                        where cru.IdUser == IdUser && cr.Status != (int)StatusWaitingRequest.DISABLE
                        select cr;
            if (model == null) return new List<CircleExchangeHouse>();
            return model.ToList();
        }
        public List<WaitingRequest> GetByCircle(ICollection<RequestInCircleExchangeHouse> requests)
        {
            var rq = from rqc in requests
                     join crq in this._context.WaitingRequests on rqc.IdWaitingRequest equals crq.Id
                     select crq.Node(rqc, crq);
            if (rq == null) return new List<WaitingRequest>();
            return rq.ToList();
        }
        public List<WaitingRequest> GetByCircle(int idCircle)
        {
            List<WaitingRequest> requests = new List<WaitingRequest>();
            var item = from rcr in this._context.RequestsInCircleExchangeHouses
                        join wr in this._context.WaitingRequests on rcr.IdWaitingRequest equals wr.Id
                        where rcr.IdCircleExchangeHouse == idCircle
                        select wr;
            if (item != null) requests.AddRange(item.ToList());
            return requests;
        }
        public List<CircleRequestViewModel> GetSuggest(int IdUser, IUserService _userService, byte[] salt, string host)
        {
            var circle = this.GetByUser(IdUser);
            List<CircleRequestViewModel> model = new List<CircleRequestViewModel>();
            foreach (var item in circle)
            {
                var node = this.GetCircleRequestDetail(item, IdUser, _userService, salt, host);
                if(node != null) model.Add(node);
            }
            return model;
        }
        public CircleRequestViewModel? GetCircleRequestDetail(CircleExchangeHouse circle, int IdUser, IUserService _userService, byte[] salt, string host)
        {
            circle.IncludeAll(this._context);
            if (circle.RequestInCircles != null)
            {
                var rq = this.GetByCircle(circle.RequestInCircles);
                if (rq.Count() > 0)
                {
                    foreach (var itemRQ in rq)
                    {
                        itemRQ.IncludeAll(this._context);
                    }
                    WaitingRequest? myNode = rq.Where(m => m.IdUser == IdUser).FirstOrDefault();
                    if (myNode != null)
                    {
                        if (myNode.Houses != null)
                        {
                            myNode.Houses.IncludeAll(this._context);
                            var inputMyNode = this.CircleRequestDetail(myNode, _userService, salt, host);
                            WaitingRequest? prevNode = rq.Where(m => m.IdUser != IdUser && m.IdCity == myNode.Houses.IdCity.Value).FirstOrDefault();
                            if (prevNode != null && inputMyNode != null)
                            {
                                prevNode.Houses.IncludeAll(this._context);
                                var inputPrevNode = this.CircleRequestDetail(prevNode, _userService, salt, host);
                                WaitingRequest? nextNode = rq.Where(m => m.IdUser != IdUser && myNode.IdCity == m.Houses.IdCity.Value).FirstOrDefault();
                                if (nextNode != null && inputPrevNode != null)
                                {
                                    nextNode.Houses.IncludeAll(this._context);
                                    var inputNextNode = this.CircleRequestDetail(nextNode, _userService, salt, host);
                                    if (inputNextNode != null)
                                    {
                                        return new CircleRequestViewModel(inputPrevNode, inputMyNode, inputNextNode, circle, IdUser);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }

    public interface ICircleRequestService
    {
        public List<WaitingRequestForSearch> GetInitWaitingRequest();
        public void Save(List<List<WaitingRequestForSearch>> requests);
        public void RemoveCircleSwap();
        public bool Remove(int idRequest);
        public void RemoveRequestOutDate();
        public void AddRange();
        public void AddNode();
        public void Update(RequestInCircleExchangeHouse request);
        public void Update(CircleExchangeHouse request);
        public List<List<WaitingRequestForSearch>> IsExist(List<List<WaitingRequestForSearch>> waitingRequestForSearches);
        public CircleExchangeHouse? GetById(int idRequest);
        public RequestInCircleExchangeHouse? GetWaitingRequestById(int idRequest, int idUser);
        public CircleRequestDetail? CircleRequestDetail(WaitingRequest request, IUserService userService, byte[] salt, string host);
        public List<CircleExchangeHouse> GetByUser(int IdUser);
        public List<WaitingRequest> GetByCircle(ICollection<RequestInCircleExchangeHouse> requests);
        public List<WaitingRequest> GetByCircle(int idCircle);
        public List<CircleRequestViewModel> GetSuggest(int IdUser, IUserService _userService, byte[] salt, string host);
        public CircleRequestViewModel? GetCircleRequestDetail(CircleExchangeHouse circle, int IdUser, IUserService _userService, byte[] salt, string host);
    }
}
