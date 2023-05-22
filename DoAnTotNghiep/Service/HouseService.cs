using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.TrainModels;
using DoAnTotNghiep.ViewModels;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.WebSockets;
using static QRCoder.PayloadGenerator;

namespace DoAnTotNghiep.Service
{
    public class HouseService : IHouseService
    {
        private DoAnTotNghiepContext _context;
        public HouseService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }
        public bool SaveHouse(House house)
        {
            try
            {
                this._context.Houses.Add(house);
                this._context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "SaveHouse_IdUser_" + house.IdUser);
            }
            return false;
        }
        public bool CheckAnyValidRequest(House house)
        {
            if(house.Requests == null && !this._context.Entry(house).Collection(m => m.Requests).IsLoaded)
            {
                this._context.Entry(house).Collection(m => m.Requests).Load();
            }
            DateTime now = DateTime.Now;
            if (house.Requests != null
                    && house.Requests.Any(m => (StatusRequestStr.IsUnValidHouse(m.Status))
                                            && DateTime.Compare(m.StartDate, now) <= 0 && DateTime.Compare(m.EndDate, now) >= 0))
            {
                return true;
            }
            return false;
        }
        public bool CheckAnyValidRequest(House house, DateTime startDate, DateTime endDate)
        {
            return house.Requests != null
                    && house.Requests.Any(m => (StatusRequestStr.IsUnValidHouse(m.Status))
                                            && DateTime.Compare(m.StartDate, startDate) <= 0 && DateTime.Compare(m.EndDate, endDate) >= 0);
        }
        public bool UpdateHouse(House house)
        {
            try
            {
                this._context.Houses.Update(house);
                this._context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "UpdateHouse_IdUser_" + house.IdUser);
            }
            return false;
        }
        public NewModelTrainInput? GetPredict(int Id)
        {
            var house = this._context.Houses.FirstOrDefault(m => m.Id == Id);
            if (house == null) return null;
            house.IncludeAll(this._context);
            try
            {
                string city = StringHelper.RemoveSyntax(StringHelper.RemoveVietnamese(house.Citys.Name.ToLower()));
                NewModelTrainInput trainInput = new NewModelTrainInput()
                {
                    Address = city,
                    Capacity = house.People,
                    Distance = (float) DistanceHelper.DistanceTo(house.Lat, house.Lng, house.Citys.Lat, house.Citys.Lng),
                    Area = (float) house.Area,
                    Rating = (float) house.Rating * 20
                };
                return trainInput;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
        public House? GetHouseByIdDiffStatus(int idUser, int id, int status)
        {
            return this._context.Houses
                                .Include(m => m.Users).ThenInclude(u => u.Files)
                                .Where(m => m.IdUser == idUser
                                            && m.Id == id
                                            && m.Status != status)
                                .FirstOrDefault();
        }
        public House? GetHouseByIdStatus(int id, int status)
        {
            return this._context.Houses
                                .Include(m => m.Users).ThenInclude(u => u.Files)
                                .FirstOrDefault(m => m.Id == id && m.Status == status);
        }
        public House? GetHouseByIdWithUser(int id, int idUser)
        {
            return this._context.Houses
                                .Include(m => m.Users).ThenInclude(u => u.Files)
                                .FirstOrDefault(m => m.Id == id && m.Users != null && m.IdUser == idUser);
        }
        public List<House> GetListHouseByUser(int idUser)
        {
            return this._context.Houses
                                .Where(m => m.Status == (int)StatusHouse.VALID
                                                    && m.IdUser == idUser)
                                .ToList();
        }
        public List<House> GetListHouseByPagination(Pagination pagination)
        {
            return this._context.Houses
                                .Skip(pagination.Page * pagination.Limit)
                                .Take(pagination.Limit)
                                .OrderByDescending(m => m.Rating)
                                .Where(m => m.Status == (int)StatusHouse.VALID)
                                .ToList();
        }
        public List<House> GetListHouseByFilter(Filter filter)
        {
            List<House> model = new List<House>();
            if (filter.DateStart != null && filter.DateEnd != null)
            {
                var res = this._context.Houses
                                .Include(m => m.Requests)
                                .Include(m => m.UtilitiesInHouses)
                                .Where(m => m.Status == (int)StatusHouse.VALID
                                            && m.IdCity == filter.IdCity
                                            && (m.Requests == null ||
                                                m.Requests != null
                                                && !m.Requests.Any(r =>
                                                (r.Status == (int)StatusRequest.ACCEPT || r.Status == (int)StatusRequest.CHECK_IN)
                                                && !(r.StartDate >= filter.DateEnd || r.EndDate <= filter.DateStart)
                                        )))
                                .ToList();
                model.AddRange(res);
            }
            else
            {
                var res = this._context.Houses
                                .Include(m => m.Requests)
                                .Include(m => m.UtilitiesInHouses)
                                .Where(m => m.Status == (int)StatusHouse.VALID
                                            && m.IdCity == filter.IdCity)
                                .ToList();
                model.AddRange(res);
            }

            if (filter.PriceStart != null)
            {
                model = model.Where(m => m.Price >= filter.PriceStart.Value).ToList();
            }
            if (filter.PriceEnd != null)
            {
                model = model.Where(m => m.Price <= filter.PriceEnd.Value).ToList();
            }

            if (filter.Utilities != null && filter.Utilities.Count() > 0)
            {
                model = model.Where(m => m.UtilitiesInHouses != null
                                        && m.UtilitiesInHouses.Where(u => u.Status == true)
                                                    .Select(m => m.IdUtilities)
                                                    .Intersect(filter.Utilities).Any()).ToList();
            }
            if (filter.People != null)
            {
                model = model.Where(m => m.People >= filter.People.Value).ToList();
            }

            switch (filter.OptionSort)
            {
                case (int)SortResult.RATING:
                    return model.OrderByDescending(m => m.Rating).ToList();
                case (int)SortResult.MIN_PRICE:
                    return model.OrderByDescending(m => m.Price).ToList();
                case (int)SortResult.MAX_PRICE:
                    return model.OrderBy(m => m.Price).ToList();
            }

            return model;
        }
        public List<House> GetListPopularHouse(int number = 10)
        {
            return this._context.Houses
                                .Include(m => m.Requests)
                                .Take(number)
                                .OrderByDescending(m => m.Rating)
                                .Where(m => m.Status == (int)StatusHouse.VALID)
                                .ToList();
        }
        public DetailHouseViewModel GetDetailHouse(House house, string host, byte[] salt)
        {
            house.IncludeAll(this._context);
            if (house.Users != null && !this._context.Entry(house.Users).Collection(m => m.Houses).IsLoaded)
            {
                this._context.Entry(house.Users).Collection(m => m.Houses).Load();
            }
            return new DetailHouseViewModel(house, salt, house.Users, host);
        }
        public List<DetailHouseViewModel> GetListDetailHouseWithManyImages(List<House> houses, IFileService fileService, string host, byte[] salt)
        {
            List<DetailHouseViewModel> res = new List<DetailHouseViewModel>();
            foreach (var item in houses)
            {
                res.Add(this.UpdateDetailHouse(this.GetDetailHouse(item, host, salt),fileService.GetImageBases(item, host)));
            }
            return res;
        }
        public List<DetailHouseViewModel> GetListDetailHouseWithOneImage(List<House> houses, IFileService fileService, string host, byte[] salt)
        {
            List<DetailHouseViewModel> res = new List<DetailHouseViewModel>();
            foreach (var item in houses)
            {
                res.Add(this.UpdateDetailHouse(this.GetDetailHouse(item, host, salt), fileService.GetImageBase(item, host)));
            }
            return res;
        }
        public List<DetailHouseViewModel> GetListDetailHouseWithManyImages(House? house, IFileService fileService, string host, byte[] salt)
        {
            List<DetailHouseViewModel> res = new List<DetailHouseViewModel>();
            if (house != null)
            {
                res.Add(this.UpdateDetailHouse(this.GetDetailHouse(house, host, salt), fileService.GetImageBases(house, host)));
            }
            return res;
        }
        public List<DetailHouseViewModel> GetListDetailHouseWithOneImage(House? house, IFileService fileService, string host, byte[] salt)
        {
            List<DetailHouseViewModel> res = new List<DetailHouseViewModel>();
            if(house != null)
            {
                res.Add(this.UpdateDetailHouse(this.GetDetailHouse(house, host, salt), fileService.GetImageBase(house, host)));
            }
            return res;
        }
        public DetailHouseViewModel UpdateDetailHouse(DetailHouseViewModel house, 
            List<ImageBase> imageBases, 
            List<DetailRatingWithUser> detailRatingWithUsers) 
        {
            this.UpdateDetailHouse(house, imageBases);
            house.Ratings.AddRange(detailRatingWithUsers);
            return house;
        }
        public DetailHouseViewModel UpdateDetailHouse(DetailHouseViewModel house,
                                            List<ImageBase> imageBases)
        {
            house.Images.AddRange(imageBases);
            return house;
        }
        public bool DeleteHouse(House house)
        {
            try
            {
                house.Status = (int)StatusHouse.DISABLE;
                this._context.Houses.Update(house);
                this._context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "DeleteHouse_IdUser_" + house.IdUser);
            }
            return false;
        }
        public int NumberHouse() => this._context.Houses
                                .Where(m => m.Status == (int)StatusHouse.VALID)
                                .ToList().Count();

        public List<int> CalRating(int idHouse)
        {
            var rate = from h in this._context.Houses
                       join f in this._context.FeedBacks on h.Id equals f.IdHouse
                       where h.Id == idHouse
                       select f;
            if(rate != null)
            {
                int number = 0;
                foreach (var item in rate.ToList())
                {
                    number += item.Rating;
                }
                int count = rate.ToList().Count();
                return new List<int>() { number,  count <= 0 ? 1: count};
            }
            return new List<int>() { 0, 1};
        }
    }

    public interface IHouseService
    {
        public bool SaveHouse(House house);
        public bool UpdateHouse(House house);
        public bool DeleteHouse(House house);
        public NewModelTrainInput? GetPredict(int Id);
        public House? GetHouseByIdDiffStatus(int idUser, int id, int status);
        public House? GetHouseByIdStatus(int id, int status);
        public House? GetHouseByIdWithUser(int id, int idUser);
        public bool CheckAnyValidRequest(House house);
        public bool CheckAnyValidRequest(House house, DateTime startDate, DateTime endDate);
        public List<House> GetListHouseByUser(int idUser);
        public List<House> GetListHouseByPagination(Pagination pagination);
        public List<House> GetListHouseByFilter(Filter filter);
        public List<House> GetListPopularHouse(int number = 10);
        public DetailHouseViewModel GetDetailHouse(House house, string host, byte[] salt);
        public List<DetailHouseViewModel> GetListDetailHouseWithManyImages(List<House> houses, IFileService fileService, string host, byte[] salt);
        public List<DetailHouseViewModel> GetListDetailHouseWithOneImage(List<House> houses, IFileService fileService, string host, byte[] salt);
        public List<DetailHouseViewModel> GetListDetailHouseWithManyImages(House? house, IFileService fileService, string host, byte[] salt);
        public List<DetailHouseViewModel> GetListDetailHouseWithOneImage(House? house, IFileService fileService, string host, byte[] salt);
        public DetailHouseViewModel UpdateDetailHouse(DetailHouseViewModel house, 
                                            List<ImageBase> imageBases, 
                                            List<DetailRatingWithUser> detailRatingWithUsers);
        public DetailHouseViewModel UpdateDetailHouse(DetailHouseViewModel house,
                                            List<ImageBase> imageBases);
        public int NumberHouse();
        public List<int> CalRating(int idHouse);
    }
}
