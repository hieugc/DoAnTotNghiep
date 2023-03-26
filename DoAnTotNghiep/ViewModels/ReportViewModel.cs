using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class ReportViewModel
    {
        public int IdHouse { get; set; } = 0;
        public string Content { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<ImageBase> Images { get; set; } = new List<ImageBase>();
    }

    public class DetailReportViewModel
    {
        public string Content { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<ImageBase> Images { get; set; } = new List<ImageBase>();
        public UserMessageViewModel User { get; set; } = new UserMessageViewModel();
    }

    public class Report
    {
        public DetailHouseViewModel House { get; set; } = new DetailHouseViewModel();
        public List<DetailReportViewModel> Details { get; set; } = new List<DetailReportViewModel>();
    }


    public class ReportItem
    {
        public DetailHouseViewModel House { get; set; } = new DetailHouseViewModel();
        public int NumberReport { get; set; } = 0;
    }
}
