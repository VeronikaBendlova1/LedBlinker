using LedBlinker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LedBlinker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Led> Leds { get; set; }


        public DbSet<Logs> Logs { get; set; }

        public DbSet<Configuration> Configurations { get; set; }    



    }
}
