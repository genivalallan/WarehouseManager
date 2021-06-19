using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WarehouseManager.Models
{
    public interface IWMRepository
    {
        IQueryable<Client> Clients { get; }
        IQueryable<Product> Products { get; }
        IQueryable<Stock> Stocks { get; }
        IQueryable<Driver> Drivers { get; }
        IQueryable<Vehicle> Vehicles { get; }
        IQueryable<Incoming> Incomings { get; }
        IQueryable<Shipping> Shippings { get; }
        IQueryable<Enhancement> Enhancements { get; }

        public bool DbInit(bool sample = false);

        public MySqlDbContext DbContext();

        public void Create(Client c);
        public void Create(Product p);
        public void Create(Stock s);
        public void Create(Driver d);
        public void Create(Vehicle v);
        public void Create(Incoming i);
        public void Create(Shipping s);
        public void Create(Enhancement e);
        
        public void Update<T>(T t);
        
        public void Delete<T>(T t);
    }
}