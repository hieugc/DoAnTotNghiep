using DoAnTotNghiep.Entity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class LocationViewModel
    {
        public List<City> Cities { get; set; } = new List<City>();
        public List<District> Districts { get; set; } = new List<District>();
        public List<Ward> Wards { get; set; } = new List<Ward>();
    }

    public class WardViewModel
    {
        public string Name { get; set; } = string.Empty;
    }
    public class DistrictViewModel
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Ward { get; set; } = new List<string>();
    }

    public class CityViewModel
    {
        public string City { get; set; } = string.Empty;
        public List<DistrictViewModel> Ward { get; set; } = new List<DistrictViewModel>();
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
