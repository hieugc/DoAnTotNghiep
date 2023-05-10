using DoAnTotNghiep.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class Filter
    {
        public string? Location { get; set; } = string.Empty;
        public int? People { get; set; }
        public int? IdCity { get; set; }//
        public int? IdDistrict { get; set; }//
        public DateTime? DateStart { get; set; }//
        public DateTime? DateEnd { get; set; }//
        public int? PriceStart { get; set; }//
        public int? PriceEnd { get; set; }//
        public List<int> Utilities { get; set; } = new List<int>();
        public int OptionSort { get; set; } = 0;
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }

    public class ExploreResult {
        public ListDetailHouses Houses { get; set; } = new ListDetailHouses();
        public Point Center { get; set; } = new Point();
        public List<Utilities> Utilities { get; set; } = new List<Utilities>();
    }
    public class Point
    {
        public double Lat { get; set; } = 0;
        public double Lng { get; set; } = 0;
    }

    public class CityAndDistrict
    {
        public CityAndDistrict() { }
        public CityAndDistrict(City city)
        {
            IdCity = city.Id;
            IdDistrict = null;
            CityName = city.Name;
            DistrictName = null;
        }
        public int IdCity { get; set; }
        public int? IdDistrict { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string? DistrictName { get; set; } = string.Empty;
    }

    public class CityView
    {
        public CityView(City city)
        {
            this.IdCity = city.Id;
            this.CityName = city.Name;
        }
        public CityView(CityAndDistrict city)
        {
            this.IdCity = city.IdCity;
            this.CityName = city.CityName;
        }
        public int IdCity { get; set; }
        public string CityName { get; set; } = string.Empty;
    }
}
