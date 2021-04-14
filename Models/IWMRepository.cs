using System.Linq;

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

        public void Create(Client c);
        public void Create(Product p);
        public void Create(Stock s);
        public void Create(Driver d);
        public void Create(Vehicle v);
        public void Create(Incoming i);
        public void Create(Shipping s);
        public void Create(Enhancement e);
        
        public void Update(Client c);
        public void Update(Product p);
        public void Update(Stock s);
        public void Update(Driver d);
        public void Update(Vehicle v);
        public void Update(Incoming i);
        public void Update(Shipping s);
        public void Update(Enhancement e);
        
        public void Delete(Client c);
        public void Delete(Product p);
        public void Delete(Stock s);
        public void Delete(Driver d);
        public void Delete(Vehicle v);
        public void Delete(Incoming i);
        public void Delete(Shipping s);
        public void Delete(Enhancement e);
    }
}