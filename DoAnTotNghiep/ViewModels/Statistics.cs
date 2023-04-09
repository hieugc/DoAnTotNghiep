using DoAnTotNghiep.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class InputRequestStatistic
    {
        public int Year { get; set; } = DateTime.Now.Year;
        public int IdHouse { get; set; } = 0;
    }

    public class RequestStatistics
    {
        public RequestStatistics(Request request)
        {
            this.StartDate = request.StartDate;
            this.EndDate = request.EndDate;
            this.IsMoney = (request.Type == 1);
            this.Amount = request.Point;
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsMoney { get; set; }
        public int Amount { get; set; }
    }
    public class HouseStatistics
    {
        public List<RequestStatistics> Requests { get; set; } = new List<RequestStatistics>();
        public List<RequestStatistics> UseForSwap { get; set; } = new List<RequestStatistics>();
    }
}
