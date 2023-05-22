using DoAnTotNghiep.Enum;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DoAnTotNghiep.Response
{
    public static class ResponseHttp
    {
        public const string System500 = "Hệ thống đang bảo trì vui lòng quay lại sau!!";
    }

    public static class AuthorizeResponseHttp
    {
        public const string NotFoundUser = "Không tìm thấy người dùng";
        public const string InCorrectPassword = "Mật khẩu không đúng";
        public const string EmailUsed = "Email đã tồn tại";
        public const string EmailValid = "Email hợp lệ";
        public const string OtpSent = "Đã gửi mã otp";
        public const string OtpExpired = "Mã otp quá hạn";
        public const string OtpInValid = "Mã otp không hợp lệ";
    }
}
