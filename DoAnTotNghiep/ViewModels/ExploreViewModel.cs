﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class Filter
    {
        public string? Location { get; set; }
        public int? People { get; set; }
        public int? IdCity { get; set; }//
        public DateTime? DateStart { get; set; }//
        public DateTime? DateEnd { get; set; }//
        public int? PriceStart { get; set; }//
        public int? PriceEnd { get; set; }//
        public List<int> Utilities { get; set; } = new List<int>();
        public int OptionSort { get; set; } = 0;//
        public int Page { get; set; } = 1;//
        public int Limit { get; set; } = 10;//
    }

    public class ExploreResult {
        public ListDetailHouses Houses { get; set; } = new ListDetailHouses();
        public Point Center { get; set; } = new Point();
    }
    public class Point
    {
        public double Lat { get; set; } = 0;
        public double Lng { get; set; } = 0;
    }
}
