using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class Filter
    {
        public string? Location { get; set; }
        public int? IdCity { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? PriceStart { get; set; }
        public int? PriceEnd { get; set; }
        public List<int> Utilities { get; set; } = new List<int>();
        public int OptionSort { get; set; } = 0;

    }
    public class ExploreResult {
        public List<DetailHouseViewModel> Houses { get; set; } = new List<DetailHouseViewModel>();
        public Point Center { get; set; } = new Point();
    }
    public class Point
    {
        public double Lat { get; set; } = 0;
        public double Lng { get; set; } = 0;
    }
}
