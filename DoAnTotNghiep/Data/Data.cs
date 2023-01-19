using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Entity;

namespace DoAnTotNghiep.Data
{
    public class DoAnTotNghiepContext: DbContext
    {
        public DoAnTotNghiepContext(DbContextOptions<DoAnTotNghiepContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<House> Houses { get; set; }
    }
}
