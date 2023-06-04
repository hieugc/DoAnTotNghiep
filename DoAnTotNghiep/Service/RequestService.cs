using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static QRCoder.PayloadGenerator;

namespace DoAnTotNghiep.Service
{
    public class RequestService : IRequestService
    {
        private DoAnTotNghiepContext _context;
        public RequestService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }
        public void SaveRequest(Request request)
        {
            this._context.Requests.Add(request);
            this._context.SaveChanges();
        }
        public void SaveRequest(List<Request> request)
        {
            this._context.Requests.AddRange(request);
            this._context.SaveChanges();
        }
        public void UpdateRequest(Request request)
        {
            this._context.Requests.Update(request);
            this._context.SaveChanges();
        }
        public void UpdateRequest(List<Request> request)
        {
            this._context.Requests.UpdateRange(request);
            this._context.SaveChanges();
        }
        public Request? GetRequestById(int Id)
        {
            return this._context.Requests.FirstOrDefault(m => m.Id == Id && m.Status != (int)StatusRequest.DISABLE);
        }
        public Request? GetRequestById(int Id, int status) => this._context.Requests.FirstOrDefault(m => m.Id == Id && m.Status == status);
        public Request? GetRequestByIdWithStartTime(int Id, int status)
        {
            DateTime now = DateTime.Now;
            return this._context.Requests
                            .Where(m => m.Id == Id
                                        && m.Status >= status
                                        && DateTime.Compare(now, m.StartDate.AddDays(-2)) >= 0)
                            .FirstOrDefault();
        }
        public Request? GetRequestByIdWithEndTime(int Id, int status)
        {
            DateTime now = DateTime.Now;
            var model = this._context.Requests
                                    .Where(m => m.Id == Id
                                                && m.Status >= status)
                                    .FirstOrDefault();
            if(model != null && DateTime.Compare(now, model.EndDate.AddHours(-6)) >= 0)
            {
                return model;
            }
            return null;
        }
        public Request? GetRequestByFeedBack(int idRequest, int idUser)
        {
            return this._context.Requests
                                        .Include(m => m.CheckOuts)
                                        .Where(m => m.Id == idRequest
                                                    && (m.Status == (int)StatusRequest.CHECK_OUT ||
                                                        m.Status == (int)StatusRequest.CHECK_IN
                                                        && m.CheckOuts != null
                                                        && m.CheckOuts.Any(c => c.IdUser == idUser)))
                                        .FirstOrDefault();
        }
        public Request? GetRequestByIdWithLessStatus(int Id, int status) => this._context.Requests.FirstOrDefault(m => m.Id == Id && (m.Status <= (int)StatusRequest.CHECK_IN));
        public List<Request> GetAllSent(int IdUser)
        {
            return this._context.Requests.OrderByDescending(m => m.StartDate).Where(m => m.IdUser == IdUser && m.Status != (int)StatusRequest.DISABLE).ToList();
        }
        public List<Request> GetValidRequestByUser(int IdUser)
        {
            return this._context.Requests
                            .Include(m => m.Houses)
                            .OrderByDescending(m => m.StartDate)
                            .Where(m => (m.Status == (int)StatusRequest.ACCEPT
                                    || m.Status == (int)StatusRequest.CHECK_IN
                                    || m.Status == (int)StatusRequest.WAIT_FOR_SWAP)
                                && (m.IdUser == IdUser ||
                                m.Houses != null && m.Houses.IdUser == IdUser))
                            .ToList();
        }
        public List<Request> GetValidReceivedRequestByUser(int IdUser)
        {
            return this._context.Requests
                                .Include(m => m.Houses)
                                .OrderByDescending(m => m.StartDate)
                                .Where(m => (m.Status == (int)StatusRequest.ACCEPT
                                                || m.Status == (int)StatusRequest.CHECK_IN
                                                || m.Status == (int)StatusRequest.CHECK_OUT
                                                || m.Status == (int)StatusRequest.ENDED)
                                            && (m.IdUser == IdUser || m.Houses != null && m.Houses.IdUser == IdUser))
                                .ToList();
        }
        public List<DetailRequest> GetValidRequestByUser(List<House> houses, IFileService _fileService, IHouseService _houseService, IFeedBackService _feedBackService, int? Status, byte[] salt, string host, int IdUser = 0)
        {
            List<DetailRequest> model = new List<DetailRequest>();
            foreach (var item in houses)
            {
                model.AddRange(this.GetValidRequestByUser(this.GetRequestByHouse(item.Id, IdUser), _fileService, _houseService, _feedBackService, Status, salt, host, IdUser));
            }
            return model;
        }
        public List<Request> GetWaitingRequestByHouse(int IdHouse, int IdUser)
        {
            var model = from h in this._context.Houses
                        join rq in this._context.Requests on h.Id equals rq.IdHouse
                        where h.Id == IdHouse && h.IdUser == IdUser && rq.Status == (int)StatusRequest.WAIT_FOR_SWAP
                        select rq;
            return model.OrderByDescending(m => m.StartDate).ToList();
        }
        public List<Request> GetRequestByHouse(int IdHouse, int IdUser)
        {
            var model = from h in this._context.Houses
                        join rq in this._context.Requests on h.Id equals rq.IdHouse
                        where h.Id == IdHouse && h.IdUser == IdUser && rq.Status != (int)StatusRequest.DISABLE
                        select rq;
            return model.OrderByDescending(m => m.StartDate).ToList();
        }
        public List<DetailRequest> GetValidRequestByUser(List<Request> requests, IFileService _fileService, IHouseService _houseService, IFeedBackService _feedBackService, int? Status, byte[] salt, string host, int IdUser = 0)
        {
            List<DetailRequest> model = new List<DetailRequest>();
            if (IdUser != 0)
            {
                foreach (var item in requests)
                {
                    DetailRequest? request = this.CreateDetailRequest(item, _fileService, _houseService, _feedBackService, Status, salt, host, IdUser);
                    if (request != null) model.Add(request);
                }
            }
            return model;
        }
        public DetailRequest? CreateDetailRequest(Request item, IFileService _fileService, IHouseService _houseService, IFeedBackService _feedBackService, int? Status, byte[] salt, string host, int IdUser)
        {
            item.IncludeAll(this._context);
            item.CheckStatus(item, IdUser);
            if (Status != null && item.Status != Status)
            {
                return null;
            }
            if (item.Houses != null)
            {
                item.Houses.IncludeAll(this._context);
                if (item.Houses.Users != null)
                {
                    item.Houses.Users.InCludeAll(this._context);
                    DetailHouseViewModel house = _houseService.UpdateDetailHouse(
                                                _houseService.GetDetailHouse(item.Houses, host, salt),
                                                _fileService.GetImageBase(item.Houses, host));
                    DetailHouseViewModel? swapHouse = null;
                    DetailRatingViewModel? userRating = _feedBackService.GetRatingByRequestDiffUser(item.FeedBacks, IdUser);
                    DetailRatingViewModel? myRating = _feedBackService.GetRatingByRequest(item.FeedBacks, IdUser);
                    if (item.SwapHouses != null)
                    {
                        swapHouse = _houseService.UpdateDetailHouse(
                                            _houseService.GetDetailHouse(item.SwapHouses, host, salt),
                                            _fileService.GetImageBase(item.SwapHouses, host));
                    }
                    DetailRequestViewModel request = new DetailRequestViewModel(item, (item.IdUser == IdUser), salt, host);
                    return new DetailRequest(house, request, swapHouse, userRating, myRating);
                }
            }
            return null;
        }
        public List<NotifyRequest> GetNotifyRequests(List<Request> requests, IFileService _fileService, IHouseService _houseService, byte[] salt, string host)
        {
            List<NotifyRequest> model = new List<NotifyRequest>();
            foreach (var item in requests)
            {
                item.IncludeAll(this._context);
                if (item.Users != null)
                {
                    DetailHouseViewModel? swapHouse = null;
                    if (item.SwapHouses != null)
                    {
                        swapHouse = _houseService.UpdateDetailHouse(
                                            _houseService.GetDetailHouse(item.SwapHouses, host, salt),
                                            _fileService.GetImageBase(item.SwapHouses, host));
                    }
                    DetailRequestViewModel request = new DetailRequestViewModel(item, false, salt, host);

                    model.Add(new NotifyRequest()
                    {
                        Request = request,
                        SwapHouse = swapHouse
                    });
                }
            }
            return model;
        }
        public List<RequestStatistics> StatisticRequestToHouse(InputRequestStatistic input, int IdUser)
        {
            return this._context.Requests
                                        .Include(m => m.Houses)
                                        .Where(m => m.IdHouse == input.IdHouse
                                                    && m.Houses != null && m.Houses.IdUser == IdUser
                                                    && (m.Status == (int)StatusRequest.ENDED || m.Status == (int)StatusRequest.CHECK_OUT)
                                                    && (m.StartDate.Year == input.Year || m.EndDate.Year == input.Year)
                                        )
                                        .Select(m => new RequestStatistics(m))
                                        .ToList();
        }
        public List<RequestStatistics> StatisticHouseUseForSwap(InputRequestStatistic input, int IdUser)
        {
            return this._context.Requests
                                        .Where(m => m.IdSwapHouse != null
                                                    && m.IdSwapHouse == input.IdHouse
                                                    && m.IdUser == IdUser
                                                    && (m.StartDate.Year == input.Year || m.EndDate.Year == input.Year)
                                        )
                                        .Select(m => new RequestStatistics(m))
                                        .ToList();
        }
        public List<Request> All() => this._context.Requests.OrderByDescending(m => m.StartDate).ToList();
    }

    public interface IRequestService
    {
        public void SaveRequest(Request request);
        public void SaveRequest(List<Request> request);
        public void UpdateRequest(Request request);
        public void UpdateRequest(List<Request> request);
        public Request? GetRequestById(int Id);
        public Request? GetRequestById(int Id, int status);
        public Request? GetRequestByIdWithStartTime(int Id, int status);
        public Request? GetRequestByIdWithEndTime(int Id, int status);
        public Request? GetRequestByIdWithLessStatus(int Id, int status);
        public Request? GetRequestByFeedBack(int idRequest, int idUser);
        public List<Request> GetValidRequestByUser(int IdUser);
        public List<Request> GetValidReceivedRequestByUser(int IdUser);
        public List<Request> GetWaitingRequestByHouse(int IdHouse, int IdUser);
        public List<Request> GetRequestByHouse(int IdHouse, int IdUser);
        public List<Request> GetAllSent(int IdUser);
        public List<Request> All();
        public List<NotifyRequest> GetNotifyRequests(List<Request> requests, IFileService _fileService, IHouseService _houseService, byte[] salt, string host);
        public List<DetailRequest> GetValidRequestByUser(List<Request> requests, IFileService _fileService, IHouseService _houseService, IFeedBackService _feedBackService, int? Status, byte[] salt, string host, int IdUser = 0);
        public List<DetailRequest> GetValidRequestByUser(List<House> houses, IFileService _fileService, IHouseService _houseService, IFeedBackService _feedBackService, int? Status, byte[] salt, string host, int IdUser = 0);
        public DetailRequest? CreateDetailRequest(Request item, IFileService _fileService, IHouseService _houseService, IFeedBackService _feedBackService, int? Status, byte[] salt, string host, int IdUser);
        public List<RequestStatistics> StatisticRequestToHouse(InputRequestStatistic input, int IdUser);
        public List<RequestStatistics> StatisticHouseUseForSwap(InputRequestStatistic input, int IdUser);
    }
}
