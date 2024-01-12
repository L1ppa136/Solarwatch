using Microsoft.EntityFrameworkCore;
using SolarWatch.Model;

namespace SolarWatch.Data
{
    public class SolarwatchDbContext : DbContext
    {
        public DbSet<City> Cities {get; set; }
        public DbSet<Twilight> TwilightData { get; set; }

        public SolarwatchDbContext(DbContextOptions<SolarwatchDbContext> options) : base(options)
        {
        }
    }
}
