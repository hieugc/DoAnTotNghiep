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
}
