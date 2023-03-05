﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Entity;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace DoAnTotNghiep.Data
{
    public class DoAnTotNghiepContext: DbContext
    {
        public DoAnTotNghiepContext(DbContextOptions<DoAnTotNghiepContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\local;Initial Catalog=DoAnTotNghiep;" +
                "Persist Security Info=True;User ID=doantotnghiep;" +
                "Password=12345678;MultipleActiveResultSets=True");
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
        public DbSet<FileInUserReport> FilesOfUsers { get; set; }
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
    }
}
