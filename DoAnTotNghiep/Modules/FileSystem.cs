using DoAnTotNghiep.Enum;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DoAnTotNghiep.Modules
{
    public static class FileSystem
    {
        public static Entity.File? SaveFileWithBase64(ImageBase imageBase, string rootPath)
        {
            string folder = Path.Combine("Uploads", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
            string uploadsFolder = Path.Combine(rootPath, "wwwroot", folder);
            return FileSystem.SaveFileWithBase64(imageBase, folder, uploadsFolder);
        }
        public static Entity.File? SaveFileWithBase64(ImageBase imageBase, string folder, string uploadsFolder)
        {
            try
            {
                string[] arr = imageBase.Data.Split("base64,");
                string ext = imageBase.Name.Split(".").Last();
                var bytes = Convert.FromBase64String(arr[1]);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string ImagePath = Guid.NewGuid().ToString() + ".png";
                string filePath = Path.Combine(uploadsFolder, ImagePath);
                System.IO.File.WriteAllBytes(filePath, bytes);
                return new Entity.File() { FileName = ImagePath, PathFolder = folder };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        public static Entity.File? SaveFileWithFormFile(IFormFile file, string rootPath)
        {
            string folder = Path.Combine("Uploads", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
            string uploadsFolder = Path.Combine(rootPath, "wwwroot", folder);
            return FileSystem.SaveFileWithFormFile(file, folder, uploadsFolder);
        }
        public static Entity.File? SaveFileWithFormFile(IFormFile file, string folder, string uploadsFolder)
        {
            try
            {
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string ImagePath = Guid.NewGuid().ToString() + ".png";
                string filePath = Path.Combine(uploadsFolder, ImagePath);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fs);
                }
                return new Entity.File() { FileName = ImagePath, PathFolder = folder };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (!Directory.Exists(filePath))
                {
                    return false;
                }
                System.IO.File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        public static bool DeleteFile(Entity.File file, string rootPath)
        {
            string uploadsFolder = Path.Combine(rootPath, "wwwroot", file.PathFolder);
            string filePath = Path.Combine(uploadsFolder, file.FileName);
            return FileSystem.DeleteFile(filePath);
        }
        public static void WriteExceptionFile(string exception, string subject)
        {
            try
            {
                DateTime now = DateTime.Now;
                string folder = Path.Combine(now.ToString("yyyy"), now.ToString("MM"), now.ToString("dd"), subject);
                string uploadsFolder = Path.Combine(@"Exception", folder);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string ImagePath = Guid.NewGuid().ToString() + ".txt";
                string filePath = Path.Combine(uploadsFolder, ImagePath);

                byte[] info = new UTF8Encoding(true).GetBytes(exception);
                System.IO.File.WriteAllBytes(filePath, info);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
