using DominosDriverHustleComp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace DominosDriverHustleComp.Server.Data
{
    public class HustleCompContext : DbContext
    {
        private readonly IConfiguration _config;

        public HustleCompContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(_config.GetConnectionString("SqliteDB"));
        }

        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliverySummary> DeliverySummaries { get; set; }
        public DbSet<WeeklySummary> WeeklySummaries { get; set; }
        public DbSet<Settings> Settings { get; set; }
    }
}
