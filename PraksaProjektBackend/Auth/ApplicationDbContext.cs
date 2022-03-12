using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PraksaProjektBackend.Models;

namespace PraksaProjektBackend.Auth
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.SeedUsers(builder);
            this.SeedRoles(builder);
            this.SeedUserRoles(builder);
        }

        private void SeedUsers(ModelBuilder builder)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                FirstName = "Administ",
                LastName = "Adi",
                Address = "Mostarska",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LockoutEnabled = false,
                PhoneNumber = "1234567890"
            };
            var password = new PasswordHasher<ApplicationUser>();
            var hashed = password.HashPassword(user, "Admin123?");
            user.PasswordHash = hashed;

            builder.Entity<ApplicationUser>().HasData(user);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = "b3383add-afff-4c1a-9468-f807bcb74057", Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() { Id = "e89f0ce1-1e63-4344-8712-8a9df022c434", Name = "Customer", ConcurrencyStamp = "2", NormalizedName = "Customer" },
                new IdentityRole() { Id = "f371b4fd-6836-4719-a8f8-b3258a75cb98", Name = "Organizer", ConcurrencyStamp = "3", NormalizedName = "Organizer" }
                );
        }

        private void SeedUserRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = "b3383add-afff-4c1a-9468-f807bcb74057", UserId = "b74ddd14-6340-4840-95c2-db12554843e5" }
                );
        }

        public DbSet<PraksaProjektBackend.Models.City> City { get; set; }

        public DbSet<PraksaProjektBackend.Models.Venue> Venue { get; set; }

    }
}
