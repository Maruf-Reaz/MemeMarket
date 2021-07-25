using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MemeMarketeerApp.Models.Common.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MemeMarketeerApp.Models;

namespace MemeMarketeerApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            modelBuilder.Entity<ApplicationRole>().HasData(
                new ApplicationRole { Id = "a682b56a-6135-4111-a0k0-bdec547e3waz", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "da9a3b0e-8b6f-8529-71d0-4fd255e23f15", CreatedOn = Convert.ToDateTime("2020-01-01"), Description = "Has All Permissions" }

             );
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser() { Id = "8ab6ee61-f36c-41b1-ae27-dbb23cbfb507", UserName = "Admin", NormalizedUserName = "ADMIN", Email = "test@mail.com", NormalizedEmail = "TEST@MAIL.COM", PasswordHash = "AQAAAAEAACcQAAAAEOJVsHUa611Khzkcg/zXgZ8EeegKhZAyW2eVPMzWJiToPuR45aBwuID99TNJ91JPxg==", SecurityStamp = "5TDMS5CNA2GYJK2URB4GDOCQI2NI7EHJ", ConcurrencyStamp = "26d21881-0a3a-44ab-aa4d-10664ace1e2d" }


             );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { UserId = "8ab6ee61-f36c-41b1-ae27-dbb23cbfb507", RoleId = "a682b56a-6135-4111-a0k0-bdec547e3waz" }
             );

            modelBuilder.Entity<PostType>().HasData(
           new PostType { Id = 1, Name = "Post Only" },
           new PostType { Id = 2, Name = "Story Only" },
           new PostType { Id = 3, Name = "Post + Story" }
        );



        }
        public DbSet<Affiliate> Affiliates { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<PostType> PostTypes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Catagory> Catagories { get; set; }
        public DbSet<AffiliateLanguage> AffiliateLanguages { get; set; }





    }
}
