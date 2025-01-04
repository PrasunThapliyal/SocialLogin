using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialLogin.Database.Model;

namespace SocialLogin.Database
{
    //public class AppDbContext : DbContext
    //{
    //    public DbSet<User> Users { get; set; }
    //    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    //}

    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
