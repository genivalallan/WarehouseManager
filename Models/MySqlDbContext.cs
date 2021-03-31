using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WarehouseManager.Models
{
    public class MySqlDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Enhancement> Enhancements { get; set; }
        public DbSet<Incoming> Incomings { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<Stock> Stocks  { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        private readonly IConfiguration configuration;

        public MySqlDbContext(IConfiguration config) => configuration = config;

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseMySQL(configuration["MySqlConnectionString"]);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Enhancement>(e =>
            {
                e.HasOne(d => d.BaseStock).WithMany(p => p.EnhancementBaseStocks);
                e.HasOne(d => d.FinalStock).WithMany(p => p.EnhancementFinalStocks);
            });
        }
    }
}