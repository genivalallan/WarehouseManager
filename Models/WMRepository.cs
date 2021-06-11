using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WarehouseManager.Models
{
    public class WMRepository : IWMRepository
    {
        private readonly MySqlDbContext dbContext;

        public WMRepository(MySqlDbContext context) => dbContext = context;

        public IQueryable<Client> Clients => dbContext.Clients;
        public IQueryable<Product> Products => dbContext.Products;
        public IQueryable<Stock> Stocks => dbContext.Stocks;
        public IQueryable<Driver> Drivers => dbContext.Drivers;
        public IQueryable<Vehicle> Vehicles => dbContext.Vehicles;
        public IQueryable<Incoming> Incomings => dbContext.Incomings;
        public IQueryable<Shipping> Shippings => dbContext.Shippings;
        public IQueryable<Enhancement> Enhancements => dbContext.Enhancements;

        public bool DbInit(bool sample = false)
        {
            if (dbContext.Database.EnsureCreated())
            {
                if (sample)
                {
                    WMDbSample.Sample(dbContext);
                }
                return true;
            }

            return false;
        }

        public void Create(Client c)
        {
            dbContext.Clients.Add(c);
            dbContext.SaveChanges();
        }

        public void Create(Driver d)
        {
            dbContext.Drivers.Add(d);
            dbContext.SaveChanges();
        }

        public void Create(Enhancement e)
        {
            if (e.BaseStockID == e.FinalStockID)
                throw new ArgumentException("Destination stock equals base stock.");
            
            string invalidArguments = "";

            Stock baseStock = dbContext.Stocks.Find(e.BaseStockID);
            if (baseStock == null) invalidArguments = "BaseStockID";

            Stock finalStock = dbContext.Stocks.Find(e.FinalStockID);
            if (finalStock == null)
                invalidArguments += ((invalidArguments != "") ? "," : "") + "FinalStockID";

            Vehicle vehicle = dbContext.Vehicles.Find(e.VehicleID);
            if (vehicle == null)
                invalidArguments += ((invalidArguments != "") ? "," : "") + "VehicleID";

            if (invalidArguments != "")
            {
                throw new ArgumentException("Invalid entity ID reference.", invalidArguments);
            }
            
            if (e.GrossWeight < vehicle.Tare )
            {
                throw new ArgumentException("Gross weight less than vehicle tare.");
            }

            e.NetWeight = e.GrossWeight - vehicle.Tare;
            if (e.NetWeight > baseStock.Balance)
                throw new ArgumentException("Net weight greater than stock balance.");

            baseStock.Balance -= e.NetWeight;
            finalStock.Balance += e.NetWeight;
            e.CreatedAt = DateTime.Now;

            dbContext.Enhancements.Add(e);
            dbContext.Stocks.Update(baseStock);
            dbContext.Stocks.Update(finalStock);
            dbContext.SaveChanges();
        }

        public void Create(Incoming i)
        {
            string invalidArguments = "";

            Stock stock = dbContext.Stocks.Find(i.StockID);
            if (stock == null) invalidArguments = "StockID";

            Vehicle vehicle = dbContext.Vehicles.Find(i.VehicleID);
            if (vehicle == null)
                invalidArguments += ((invalidArguments != "") ? "," : "" ) + "VehicleID";

            if (invalidArguments != "")
            {
                throw new ArgumentException("Invalid entity ID reference.", invalidArguments);
            }

            if (i.GrossWeight < vehicle.Tare)
            {
                throw new ArgumentException("Gross weight less than vehicle tare.");
            }

            i.NetWeight = i.GrossWeight - vehicle.Tare;
            stock.Balance += i.NetWeight;
            i.CreatedAt = DateTime.Now;

            dbContext.Incomings.Add(i);
            dbContext.Stocks.Update(stock);
            dbContext.SaveChanges();
        }

        public void Create(Product p)
        {
            dbContext.Products.Add(p);
            dbContext.SaveChanges();
        }

        public void Create(Shipping s)
        {
            string invalidArguments = "";

            Stock stock = dbContext.Stocks.Find(s.StockID);
            if (stock == null) invalidArguments = "StockID";

            Vehicle vehicle = dbContext.Vehicles.Find(s.VehicleID);
            if (vehicle == null)
                invalidArguments += ((invalidArguments != "") ? "," : "") + "VehicleID";

            if (invalidArguments != "")
            {
                throw new ArgumentException("Invalid entity ID reference.", invalidArguments);
            }

            if (s.GrossWeight < vehicle.Tare)
            {
                throw new ArgumentException("Gross weight less than vehicle tare.");
            }

            s.NetWeight = s.GrossWeight - vehicle.Tare;
            if (s.NetWeight > stock.Balance)
                throw new ArgumentException("Net weight greater than stock balance.");
            
            stock.Balance -= s.NetWeight;
            s.CreatedAt = DateTime.Now;

            dbContext.Shippings.Add(s);
            dbContext.Stocks.Update(stock);
            dbContext.SaveChanges();
        }

        public void Create(Stock s)
        {
            dbContext.Stocks.Add(s);
            dbContext.SaveChanges();
        }

        public void Create(Vehicle v)
        {
            dbContext.Vehicles.Add(v);
            dbContext.SaveChanges();
        }

        public void Update<T>(T t)
        {
            switch (t)
            {
                case Client c:
                    dbContext.Clients.Update(c);
                    break;
                case Driver d:
                    dbContext.Drivers.Update(d);
                    break;
                case Enhancement e:
                    dbContext.Enhancements.Update(e);
                    break;
                case Incoming i:
                    dbContext.Incomings.Update(i);
                    break;
                case Product p:
                    dbContext.Products.Update(p);
                    break;
                case Shipping s:
                    dbContext.Shippings.Update(s);
                    break;
                case Stock stk:
                    dbContext.Stocks.Update(stk);
                    break;
                case Vehicle v:
                    dbContext.Vehicles.Update(v);
                    break;
                default:
                    return;
            }

            dbContext.SaveChanges();
        }

        public void Delete<T>(T t)
        {
            switch (t)
            {
                case Client c:
                    dbContext.Clients.Remove(c);
                    break;
                case Driver d:
                    dbContext.Drivers.Remove(d);
                    break;
                case Enhancement e:
                    dbContext.Enhancements.Remove(e);
                    break;
                case Incoming i:
                    dbContext.Incomings.Remove(i);
                    break;
                case Product p:
                    dbContext.Products.Remove(p);
                    break;
                case Shipping s:
                    dbContext.Shippings.Remove(s);
                    break;
                case Stock stk:
                    dbContext.Stocks.Remove(stk);
                    break;
                case Vehicle v:
                    dbContext.Vehicles.Remove(v);
                    break;
                default:
                    return;
            }

            dbContext.SaveChanges();
        }
    }

    public static class WMDbSample
    {
        public static void Sample(MySqlDbContext dbContext)
        {
            Client[] clients = new Client[]
            {
                new Client { Name = "Client 1", Address = "Client Address 1" },
                new Client { Name = "Client 2", Address = "Client Address 2" },
                new Client { Name = "Client 3", Address = "Client Address 3" },
                new Client { Name = "Client 4", Address = "Client Address 4" },
                new Client { Name = "Client 5", Address = "Client Address 5" }
            };
            foreach (var item in clients)
            {
                dbContext.Clients.Add(item);
            }
            dbContext.SaveChanges();

            Product[] products = new Product[]
            {
                new Product { Name = "Product 1", Description = "Product description 1" },
                new Product { Name = "Product 2", Description = "Product description 2" },
                new Product { Name = "Product 3", Description = "Product description 3" },
            };
            foreach (var item in products)
            {
                dbContext.Products.Add(item);
            }
            dbContext.SaveChanges();

            Stock[] stocks = new Stock[]
            {
                new Stock { ProductID = products[0].ID, ClientID = clients[0].ID, Balance = 50 },
                new Stock { ProductID = products[1].ID, ClientID = clients[1].ID, Balance = 50 },
                new Stock { ProductID = products[2].ID, ClientID = clients[2].ID, Balance = 50 },
                new Stock { ProductID = products[0].ID, ClientID = clients[3].ID, Balance = 50 },
                new Stock { ProductID = products[2].ID, ClientID = clients[4].ID, Balance = 50 },
                new Stock { ProductID = products[1].ID, ClientID = clients[2].ID, Balance = 50 },
            };
            foreach (var item in stocks)
            {
                dbContext.Stocks.Add(item);
            }
            dbContext.SaveChanges();

            Driver[] drivers = new Driver[]
            {
                new Driver { Name = "Driver 1", CNH = "12345678901" },
                new Driver { Name = "Driver 2", CNH = "12345678902" },
                new Driver { Name = "Driver 3", CNH = "12345678903" },
                new Driver { Name = "Driver 4", CNH = "12345678904" }
            };
            foreach (var item in drivers)
            {
                dbContext.Drivers.Add(item);
            }
            dbContext.SaveChanges();

            Vehicle[] vehicles = new Vehicle[]
            {
                new Vehicle { Plate1 = "AB1C23", Plate2 = "", Plate3 = "", RNTRC = "01234567", Tare = 13200 },
                new Vehicle { Plate1 = "AB1C24", Plate2 = "AB2C34", Plate3 = "", RNTRC = "01234568", Tare = 14350 },
                new Vehicle { Plate1 = "AB1C25", Plate2 = "AB2C35", Plate3 = "", RNTRC = "01234569", Tare = 13900 },
                new Vehicle { Plate1 = "AB1C26", Plate2 = "AB2C36", Plate3 = "AB3C45", RNTRC = "01234560", Tare = 18150 }
            };
            foreach (var item in vehicles)
            {
                dbContext.Vehicles.Add(item);
            }
            dbContext.SaveChanges();
        }
    }
}