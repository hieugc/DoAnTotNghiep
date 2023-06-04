using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class GDP
    {
        public GDP(int number, double pourCent)
        {
            this.PourCent = pourCent;
            this.Increase = number;
        }
        public int Increase { get; set; } = 0;
        public double PourCent { get; set; } = 0.0;
    }

    public class RequestAdmin
    {
        public RequestAdmin(int total, int accept, int reject)
        {
            this.Total = total;
            this.Accept = accept;
            this.Reject = reject;
        }
        public int Total { get; set; } = 0;
        public int Accept { get; set; } = 0;
        public int Reject { get; set; } = 0;
    }
}
