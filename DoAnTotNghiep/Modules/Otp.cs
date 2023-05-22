using DoAnTotNghiep.Enum;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static System.Net.WebRequestMethods;

namespace DoAnTotNghiep.Modules
{
    public static class OtpGender
    {
        public static string RandomOTP() => new Random().Next(100000, 999999).ToString();
        public static bool SendOTP(string otp, string to, string? emailSystem, string? passwordSystem)
        {
            if(string.IsNullOrEmpty(emailSystem) || string.IsNullOrEmpty(passwordSystem)) return false;

            EmailSender sender = new EmailSender(emailSystem, passwordSystem);
            return sender.SendMail(to, Subject.SendOTP(), otp, null, string.Empty);
        }
    }
}
