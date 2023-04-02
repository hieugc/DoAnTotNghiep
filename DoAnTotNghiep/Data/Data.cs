using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Entity;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.Configuration;

namespace DoAnTotNghiep.Data
{
    public class DoAnTotNghiepContext: DbContext
    {
        private readonly IConfiguration _configuration;
        public DoAnTotNghiepContext(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DataContext"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CircleExchangeHouseOfUser>().HasKey(m => new { m.IdUser, m.IdCircleExchangeHouse });
            modelBuilder.Entity<FileInAdminReport>().HasKey(m => new { m.IdFile, m.IdAdminReport });
            modelBuilder.Entity<FileInUserReport>().HasKey(m => new { m.IdFile, m.IdUserReport });
            modelBuilder.Entity<FileInUserResponse>().HasKey(m => new { m.IdFile, m.IdUserResponse });
            modelBuilder.Entity<FileOfHouse>().HasKey(m => new { m.IdFile, m.IdHouse });
            modelBuilder.Entity<RequestInCircleExchangeHouse>().HasKey(m => new { m.IdWaitingRequest, m.IdCircleExchangeHouse });
            modelBuilder.Entity<RulesInHouse>().HasKey(m => new { m.IdRules, m.IdHouse });
            modelBuilder.Entity<UtilitiesInHouse>().HasKey(m => new { m.IdHouse, m.IdUtilities });
            modelBuilder.Entity<UsersInChatRoom>().HasKey(m => new { m.IdChatRoom, m.IdUser });
            modelBuilder.Entity<CheckOut>().HasKey(m => new { m.IdRequest, m.IdUser });
        }


        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<DoAnTotNghiep.Entity.AdminReport> AdminReports { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<CircleExchangeHouse> CircleExchangeRooms { get; set; }
        public DbSet<CircleExchangeHouseOfUser> CircleExchangeHouseOfUsers { get;set; }
        public DbSet<FeedBack> FeedBacks { get; set; }
        public DbSet<Entity.File> Files { get; set; }
        public DbSet<FileOfHouse> FilesOfHouses { get; set;}
        public DbSet<FileInAdminReport> FileInAdminReports { get; set; }
        public DbSet<FileInUserReport> FileInUserReports { get; set; }
        public DbSet<FileInUserResponse> FilesOfUsersResponses { get; set;}
        public DbSet<Message> Messages { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestInCircleExchangeHouse> RequestsInCircleExchangeHouses { get;set; }
        public DbSet<Rules> Rules { get; set; }
        public DbSet<RulesInHouse> RulesInHouses { get; set; }
        public DbSet<UserReport> UserReports { get; set; }
        public DbSet<UserResponse> UserResponses { get; set; }
        public DbSet<UsersInChatRoom> UsersInChatRooms { get; set; }   
        public DbSet<Utilities> Utilities { get; set; }
        public DbSet<UtilitiesInHouse> UtilitiesInHouse { get; set; }
        public DbSet<WaitingRequest> WaitingRequests { get; set; }
        public DbSet<CheckOut> CheckOuts { get; set; }
        public DbSet<Notification> Notifications { get; set; }  
    }
}
