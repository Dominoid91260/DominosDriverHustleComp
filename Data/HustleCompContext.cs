using DominosDriverHustleComp.Models;

using Microsoft.EntityFrameworkCore;

namespace DominosDriverHustleComp.Data
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
        public DbSet<DeliveryRecord> Deliveries { get; set; }
    }
}
