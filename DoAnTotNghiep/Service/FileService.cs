using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace DoAnTotNghiep.Service
{
    public class FileService : IFileService
    {
        private DoAnTotNghiepContext _context;
        public FileService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }
        public Entity.File? GetFileById(int Id)
        {
            return this._context.Files.Where(m => m.Id == Id).FirstOrDefault();
        }
        public void Save(Entity.File files)
        {
            this._context.Files.Add(files);
            this._context.SaveChanges();
        }
        public void Update(Entity.File files)
        {
            this._context.Files.Update(files);
            this._context.SaveChanges();
        }
        public List<Entity.File> SaveRangeFile(List<Entity.File> files)
        {
            this._context.Files.AddRange(files);
            this._context.SaveChanges();
            return files;
        }
        public void SaveRangeFileOfHouse(House house, List<Entity.File> files)
        {
            List<FileOfHouse> fileOfHouses = new List<FileOfHouse>();
            foreach (var item in files)
            {
                fileOfHouses.Add(new FileOfHouse()
                {
                    IdFile = item.Id,
                    IdHouse = house.Id
                });
            }

            this._context.FilesOfHouses.AddRange(fileOfHouses);
            this._context.SaveChanges();
        }
        public List<ImageBase> GetImageBases(House house, string host)
        {
            List<ImageBase> model = new List<ImageBase>();
            if(house.FileOfHouses != null)
            {
                foreach (var f in house.FileOfHouses)
                {
                    f.IncludeAll(this._context);
                    if (f.Files != null)
                    {
                        model.Add(new ImageBase(f.Files, host));
                    }
                }
            }
            return model;
        }
        public List<ImageBase> GetImageBase(House house, string host)
        {
            List<ImageBase> model = new List<ImageBase>();
            if (house.FileOfHouses != null)
            {
                foreach (var f in house.FileOfHouses)
                {
                    f.IncludeAll(this._context);
                    if (f.Files != null)
                    {
                        model.Add(new ImageBase(f.Files, host));
                        break;
                    }
                }
            }
            return model;
        }
        public List<Entity.File> AddFileHouse(House house, List<ImageBase?> imageBases, string rootPath)
        {
            List<Entity.File> files = new List<Entity.File>();
            foreach (var item in imageBases)
            {
                if (item != null)
                {
                    Entity.File? file = FileSystem.SaveFileWithBase64(item, rootPath);
                    if (file != null)
                    {
                        files.Add(file);
                    }
                }
            }
            return files;
        }
        public List<Entity.File> AddFileHouse(House house, IFormFileCollection formFile, string rootPath)
        {
            List<Entity.File> files = new List<Entity.File>();
            foreach (var item in formFile)
            {
                if (item != null)
                {
                    Entity.File? file = FileSystem.SaveFileWithFormFile(item, rootPath);
                    if (file != null)
                    {
                        files.Add(file);
                    }
                }
            }
            return files;
        }
        public List<Entity.File> UpdateFileHouse(House house, List<ImageBase?> imageBases, string rootPath)
        {
            List<Entity.File> files = new List<Entity.File>();
            List<int> idNotRemove = new List<int>();
            foreach (var item in imageBases)
            {
                if (item != null)
                {
                    if (!string.IsNullOrEmpty(item.Data) && (item.Id == null || item.Id == 0))
                    {
                        Entity.File? file = FileSystem.SaveFileWithBase64(item, rootPath);
                        if (file != null)
                        {
                            files.Add(file);
                        }
                    }
                    else
                    {
                        if (item.Id != null && item.Id != 0)
                        {
                            idNotRemove.Add(item.Id.Value);
                        }
                    }
                }
            }
            if (house.FileOfHouses != null && idNotRemove.Count() > 0)
            {
                var deleteFileOfHouse = house.FileOfHouses
                                            .Where(m => m.IdHouse == house.Id && !idNotRemove.Contains(m.IdFile))
                                            .ToList();
                List<int> idRemove = deleteFileOfHouse.Select(m => m.IdFile).ToList();
                List<Entity.File> deleteFiles = this._context.Files.Where(m => idRemove.Contains(m.Id)).ToList();
                this._context.Files.RemoveRange(deleteFiles);
                this._context.FilesOfHouses.RemoveRange(deleteFileOfHouse);
                this._context.SaveChanges();
            }
            return files;
        }
        public List<Entity.File> UpdateFileHouse(House house, IFormFileCollection formFile, List<int> IdRemove, string rootPath)
        {
            List<Entity.File> files = new List<Entity.File>();
            foreach (var item in formFile)
            {
                if (item != null)
                {
                    Entity.File? file = FileSystem.SaveFileWithFormFile(item, rootPath);
                    if (file != null)
                    {
                        files.Add(file);
                    }
                }
            }
            if (house.FileOfHouses != null && IdRemove.Count() > 0)
            {
                var deleteFileOfHouse = house.FileOfHouses
                                            .Where(m => m.IdHouse == house.Id
                                                        && IdRemove.Contains(m.IdFile))
                                            .ToList();
                List<Entity.File> deleteFiles = this._context.Files.Where(m => IdRemove.Contains(m.Id)).ToList();
                this._context.Files.RemoveRange(deleteFiles);
                this._context.FilesOfHouses.RemoveRange(deleteFileOfHouse);
                this._context.SaveChanges();
            }
            return files;
        }


        public User UpdateFileUser(User user, ImageBase imageBases, string rootPath)
        {
            if (user.IdFile == null)
            {
                Entity.File? file = FileSystem.SaveFileWithBase64(imageBases, rootPath);
                if (file != null)
                {
                    this.Save(file);
                    user.IdFile = file.Id;
                }
            }
            else
            {
                var file = this.GetFileById(user.IdFile.Value);
                if (file != null)
                {
                    Entity.File? saveFile = FileSystem.SaveFileWithBase64(imageBases, rootPath);
                    if (saveFile != null)
                    {
                        FileSystem.DeleteFile(file, rootPath);
                        file.PathFolder = saveFile.PathFolder;
                        file.FileName = saveFile.FileName;
                        this.Update(file);
                    }
                }
            }
            return user;
        }
        public User UpdateFileUser(User user, IFormFile formFile, string rootPath)
        {
            if (user.IdFile == null)
            {
                Entity.File? file = FileSystem.SaveFileWithFormFile(formFile, rootPath);
                if (file != null)
                {
                    this.Save(file);
                    user.IdFile = file.Id;
                }
            }
            else
            {
                Entity.File? file = this.GetFileById(user.IdFile.Value);
                if (file != null)
                {
                    Entity.File? saveFile = FileSystem.SaveFileWithFormFile(formFile, rootPath);
                    if (saveFile != null)
                    {
                        FileSystem.DeleteFile(file, rootPath);
                        file.PathFolder = saveFile.PathFolder;
                        file.FileName = saveFile.FileName;
                        this.Update(file);
                    }
                }
            }
            return user;
        }
    }

    public interface IFileService
    {
        public void Save(Entity.File files);
        public void Update(Entity.File files);
        public List<Entity.File> SaveRangeFile(List<Entity.File> files);
        public void SaveRangeFileOfHouse(House house, List<Entity.File> files);
        public List<ImageBase> GetImageBases(House house, string host);
        public List<ImageBase> GetImageBase(House house, string host);
        public Entity.File? GetFileById(int Id);
        public List<Entity.File> AddFileHouse(House house, List<ImageBase?> imageBases, string rootPath);
        public List<Entity.File> AddFileHouse(House house, IFormFileCollection formFile, string rootPath);
        public List<Entity.File> UpdateFileHouse(House house, List<ImageBase?> imageBases, string rootPath);
        public List<Entity.File> UpdateFileHouse(House house, IFormFileCollection formFile, List<int> IdRemove, string rootPath);

        public User UpdateFileUser(User user, ImageBase imageBases, string rootPath);
        public User UpdateFileUser(User user, IFormFile formFile, string rootPath);
    }
}
