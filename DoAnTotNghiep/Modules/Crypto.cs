using DoAnTotNghiep.Enum;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;

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

        public static byte[] Salt(IConfiguration _configuration)
        {
            string? key = _configuration.GetConnectionString(SystemKey.Base64());
            key = (key == null ? string.Empty : key).PadRight(32, '*');
            byte[] salt = Encoding.ASCII.GetBytes(key);
            return salt;
        }

        public static string EncodeKey(string plainText, byte[] salt)
        {
            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = salt;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

        public static string DecodeKey(string cipherText, byte[] salt)
        {
            byte[] iv = new byte[16];
            try
            {
                byte[] buffer = Convert.FromBase64String(cipherText);
                using (Aes aes = Aes.Create())
                {
                    aes.Key = salt;
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
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
