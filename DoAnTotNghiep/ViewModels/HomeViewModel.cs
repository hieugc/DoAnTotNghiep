using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class HomeViewModel
    {
        public List<DetailHouseViewModel> PopularHouses { get; set; } = new List<DetailHouseViewModel>();
        public List<PopularCityViewModel> PopularCities { get; set; } = new List<PopularCityViewModel>();
        public int NumberHouses { get; set; } = 0;
        public int NumberCities { get; set; } = 0;
    }
}
