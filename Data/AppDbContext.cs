using Dotnet_Core_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Core_Web_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> option) : base(option) { }

        public DbSet<Character> Characters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
    }
}