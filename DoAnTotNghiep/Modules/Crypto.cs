using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace DoAnTotNghiep.Modules
{
    public class Crypto
    {
        public static byte[] Salt()
        {
            return RandomNumberGenerator.GetBytes(128 / 8);
        }

        public static string SaltStr(byte[] salt)
        {
            return Convert.ToBase64String(salt);
        }

        public static byte[] Salt(string salt)
        {
            return Convert.FromBase64String(salt);
        }

        public static string HashPass(string rawPass)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: rawPass,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }
        public static string HashPass(string rawPass, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: rawPass,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }

        public static bool IsPass(string rawPass, string storedPass, string storedSalt)
        {
            byte[] salt = Convert.FromBase64String(storedSalt);
            string currentPass = HashPass(rawPass, salt);
            return currentPass == storedPass;
        }
    }
}
