using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class AdminReportViewModel
    {
        public int IdHouse { get; set; } = 0;
        public string Content { get; set; } = string.Empty;
        public DateTime Deadline { get; set; } = DateTime.Now;
        public List<ImageBase> Images { get; set; } = new List<ImageBase>();
    }

    public class DetailAdminReportViewModel
    {
        public string Content { get; set; } = string.Empty;
        public List<ImageBase> Images { get; set; } = new List<ImageBase>();
    }
}
