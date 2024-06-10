using Microsoft.EntityFrameworkCore;
using CatApp.Models;

namespace CatApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
    }
}
