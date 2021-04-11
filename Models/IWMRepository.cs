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
        public void Add<T>(T t);
        public void Update<T>(T t);
        public void Delete<T>(T t);
    }
}