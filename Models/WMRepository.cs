using System.Linq;

namespace WarehouseManager.Models
{
    public class WMRepository : IWMRepository
    {
        private MySqlDbContext dbContext;

        public WMRepository(MySqlDbContext context) => dbContext = context;

        public IQueryable<Client> Clients => dbContext.Clients;
        public IQueryable<Product> Products => dbContext.Products;
        public IQueryable<Stock> Stocks => dbContext.Stocks;
        public IQueryable<Driver> Drivers => dbContext.Drivers;
        public IQueryable<Vehicle> Vehicles => dbContext.Vehicles;
        public IQueryable<Incoming> Incomings => dbContext.Incomings;
        public IQueryable<Shipping> Shippings => dbContext.Shippings;
        public IQueryable<Enhancement> Enhancements => dbContext.Enhancements;

        public bool DbInit() => dbContext.Database.EnsureCreated();

        public void Add(Client client)
        {
            dbContext.Clients.Add(client);
            dbContext.SaveChanges();
        }
    }
}