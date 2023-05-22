using DoAnTotNghiep.Entity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class BingViewModel
    {
        public BingViewModel(District district)
        {
            this.Id = district.Id;
            this.Name = district.Name;
            this.BingName = district.BingName;
        }
        public BingViewModel(Ward ward)
        {
            this.Id = ward.Id;
            this.Name = ward.Name;
            this.BingName = ward.BingName;
        }
        public BingViewModel(City city)
        {
            this.Id = city.Id;
            this.Name = city.Name;
            this.BingName = city.BingName;
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BingName { get; set; } = string.Empty;
    }
    public class LocationViewModel
    {
        public LocationViewModel(District district)
        {
            this.Id = district.Id;
            this.Name = district.Name;
            this.Lat = district.Lat;
            this.Lng = district.Lng;
        }
        public LocationViewModel(City city)
        {
            this.Id = city.Id;
            this.Name = city.Name;
            this.Lat = city.Lat;
            this.Lng = city.Lng;
        }
        public LocationViewModel(Ward ward)
        {
            this.Id = ward.Id;
            this.Name = ward.Name;
            this.Lat = ward.Lat;
            this.Lng = ward.Lng;
        }
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; } = 0;
        public double Lat { get; set; } = 0;
        public double Lng { get; set; } = 0;
    }

    public class PopularCityViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; } = 0;
        public PointViewModel Location { get; set; } = new PointViewModel();
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

    }
    public class PointViewModel
    {
        public double Lat { get; set; } = 0;
        public double Lng { get; set; } = 0;
    }

    public class BingMapViewModel
    {
        public string BingName { get; set; } = string.Empty;
        public int Id { get; set; } = 0;
        public double Lat { get; set; } = 0;
        public double Lng { get; set; } = 0;
    }
}
