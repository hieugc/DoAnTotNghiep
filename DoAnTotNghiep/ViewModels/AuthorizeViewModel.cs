using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class RefreshToken
    {
        public string Action { get; set;} = string.Empty;
        public string? UserAccess { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string OldToken { get; set; } = string.Empty;
    }
}
