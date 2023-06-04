using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.TrainModels;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.ML;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.WebSockets;
using static QRCoder.PayloadGenerator;

namespace DoAnTotNghiep.Service
{
    public class LocationService : ILocationService
    {
        private DoAnTotNghiepContext _context;
        public LocationService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }

        public List<CityAndDistrict> GetSuggestLocation(string location)
        {
            location = StringHelper.RemoveSyntax(StringHelper.RemoveVietnamese(location));
            int number = 12;

            List<CityAndDistrict> city = this._context.Cities.ToList()
                                    .Where(m => StringHelper.RemoveVietnamese(m.Name).Contains(location))
                                    .Select(m => new CityAndDistrict(m))
                                    .Take(number).ToList();
            number -= city.Count();

            if (number > 0)
            {

                var item = from ct in this._context.Cities
                           join dt in this._context.Districts on ct.Id equals dt.IdCity
                           select new CityAndDistrict()
                           {
                               IdCity = ct.Id,
                               IdDistrict = dt.Id,
                               CityName = ct.Name,
                               DistrictName = dt.Name
                           };
                List<CityAndDistrict> model = item.ToList()
                                            .Where(m => StringHelper.RemoveVietnamese((m.DistrictName + " " + m.CityName)).Contains(location)
                                                            && !city.Any(c => c.IdCity == m.IdCity))
                                            .Take(number).ToList();
                city.AddRange(model);
            }
            return city;
        }
        public List<PopularCityViewModel> GetPopularCity(string host, int number = 10)
        {
            var cities = this._context.Cities.Include(m => m.houses).Where(m => m.houses != null && m.houses.Any(m => m.Status == (int)StatusHouse.VALID)).ToList();
            List<City> cities1 = (cities != null ? cities.ToList() : new List<City>());
            List<PopularCityViewModel> cityList = cities1
                                                .OrderByDescending(m => m.Count)
                                                .Take(number)
                                                .Select(m => new PopularCityViewModel()
                                                {
                                                    Name = m.Name,
                                                    Id = m.Id,
                                                    ImageUrl = host + "/Image/img-city.jpg",
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

        public int NumberCity() => this._context.Cities.Include(m => m.houses)
                                        .Where(m => m.houses != null
                                            && m.houses.Any(h => h.Status == (int)StatusHouse.VALID))
                                        .ToList().Count();
        public List<BingViewModel> GetBingViewModel(int option, int? id)
        {
            if(option == 1) return this._context.Cities.Select(m => new BingViewModel(m)).ToList();
            else if (option == 2) return this._context.Districts.Where(m => m.IdCity == id.Value).Select(m => new BingViewModel(m)).ToList();
            return this._context.Wards.Where(m => m.IdDistrict == id.Value).Select(m => new BingViewModel(m)).ToList();
        }
        public List<LocationViewModel> GetDistrictViewModel(int option, int? id)
        {
            if (option == 1) return this._context.Cities.Select(m => new LocationViewModel(m)).ToList();
            else if (option == 2) return this._context.Districts.Where(m => m.IdCity == id.Value).Select(m => new LocationViewModel(m)).ToList();
            return this._context.Wards.Where(m => m.IdDistrict == id.Value).Select(m => new LocationViewModel(m)).ToList();
        }
        public NewModelTrainInput? GetPredictByCity(int IdCity, double Area, int Capacity, int Rating, double Lat, double Lng)
        {
            var city = this._context.Cities.FirstOrDefault(m => m.Id == IdCity);
            if (city == null) return null;
            return new NewModelTrainInput()
            {
                Address = StringHelper.RemoveSyntax(StringHelper.RemoveVietnamese(city.Name.ToLower())),
                Capacity = Capacity,
                Distance = (float)DistanceHelper.DistanceTo(Lat, Lng, city.Lat, city.Lng),
                Area = (float)Area,
                Rating = (float)Rating * 20
            };
        }

        public List<City> AllCity() => this._context.Cities.ToList();
        public List<District> AllDistrict() => this._context.Districts.ToList();
    }

    public interface ILocationService
    {
        public NewModelTrainInput? GetPredictByCity(int IdCity, double Area, int Capacity, int Rating, double Lat, double Lng);
        public List<CityAndDistrict> GetSuggestLocation(string location);
        public List<PopularCityViewModel> GetPopularCity(string host, int number = 10);
        public List<BingViewModel> GetBingViewModel(int option, int? id);
        public List<LocationViewModel> GetDistrictViewModel(int option, int? id);
        public int NumberCity();
        public List<City> AllCity();
        public List<District> AllDistrict();
    }
}
