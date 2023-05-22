using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.WebSockets;
using static QRCoder.PayloadGenerator;

namespace DoAnTotNghiep.Service
{
    public class UtilitiesService : IUtilitiesService
    {
        private DoAnTotNghiepContext _context;
        public UtilitiesService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }
        public bool SaveUtilities(Utilities utilities)
        {
            try
            {
                this._context.Utilities.Add(utilities);
                this._context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "SaveUtilities_" + utilities.Content);
            }
            return false;
        }
        public bool UpdateUtilities(Utilities utilities)
        {
            try
            {
                this._context.Utilities.Update(utilities);
                this._context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "UpdateUtilities_" + utilities.Content);
            }
            return false;
        }
        public List<Utilities> All()
        {
            return this._context.Utilities.ToList();
        }
        public void AddUtilitiesForHouse(House house, List<int> currentUtilities)
        {
            List<UtilitiesInHouse> utilities = new List<UtilitiesInHouse>();
            foreach (var item in currentUtilities)
            {
                UtilitiesInHouse utilitiesInHouse = new UtilitiesInHouse()
                {
                    IdUtilities = item,
                    IdHouse = house.Id,
                    Status = true
                };
                utilities.Add(utilitiesInHouse);
            }
            this._context.UtilitiesInHouse.AddRange(utilities);
            this._context.SaveChanges();
        }
        public void UpdateUtilitiesForHouse(House house, List<int> currentUtilities)
        {

            if (house.UtilitiesInHouses == null)
            {
                this.AddUtilitiesForHouse(house, currentUtilities);
            }
            else
            {
                List<UtilitiesInHouse> utilitiesUpdateFalse = house.UtilitiesInHouses.Where(m => !currentUtilities.Contains(m.IdUtilities)).ToList();
                foreach (var item in utilitiesUpdateFalse)
                {
                    item.Status = false;
                }
                this._context.UtilitiesInHouse.UpdateRange(utilitiesUpdateFalse);

                List<UtilitiesInHouse> utilitiesUpdateTrue = house.UtilitiesInHouses.Where(m => currentUtilities.Contains(m.IdUtilities)).ToList();
                foreach (var item in utilitiesUpdateTrue)
                {
                    item.Status = true;
                }
                this._context.UtilitiesInHouse.UpdateRange(utilitiesUpdateTrue);

                List<int> id = house.UtilitiesInHouses.Where(m => currentUtilities.Contains(m.IdUtilities)).Select(m => m.IdUtilities).ToList();

                List<UtilitiesInHouse> utilities = new List<UtilitiesInHouse>();
                foreach (var item in currentUtilities)
                {
                    if (!id.Contains(item))
                    {
                        UtilitiesInHouse utilitiesInHouse = new UtilitiesInHouse()
                        {
                            IdUtilities = item,
                            IdHouse = house.Id,
                            Status = true
                        };
                        utilities.Add(utilitiesInHouse);
                    }
                }
                this._context.UtilitiesInHouse.AddRange(utilities);
                this._context.SaveChanges();
            }
        }
    }

    public interface IUtilitiesService
    {
        public bool SaveUtilities(Utilities utilities);
        public bool UpdateUtilities(Utilities utilities);
        public List<Utilities> All();
        public void AddUtilitiesForHouse(House house, List<int> currentUtilities);
        public void UpdateUtilitiesForHouse(House house, List<int> currentUtilities);
    }
}
