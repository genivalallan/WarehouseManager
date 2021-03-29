using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WarehouseManager.Models;

namespace WarehouseManager.Controllers
{
    public class ListController : Controller
    {
        // TODO: Pagination feature
        // TODO: Redirect if page argument is out of range
        private readonly IWMRepository repository;
        private int listSize = 5;

        public ListController(IWMRepository repo) => repository = repo;

        public IActionResult Clients(int page = 1)
        {
            var clients = repository.Clients
            .OrderBy(c => c.ID)
            .Skip((page - 1) * listSize)
            .Take(listSize);

            ViewData["listSize"] = listSize;
            ViewData["page"] = page;
            return View(clients);
        }

        public IActionResult Products(int page = 1)
        {
            var products = repository.Products
            .OrderBy(p => p.ID)
            .Skip((page - 1) * listSize)
            .Take(listSize);

            ViewData["listSize"] = listSize;
            ViewData["page"] = page;
            return View(products);
        }

        public IActionResult Stocks(int page = 1)
        {
            var stocks = repository.Stocks
            .OrderBy(s => s.ID)
            .Skip((page - 1) * listSize)
            .Take(listSize)
            .Include(s => s.Product)
            .Include(s => s.Owner);

            ViewData["listSize"] = listSize;
            ViewData["page"] = page;
            return View(stocks);
        }

        public IActionResult Drivers(int page = 1)
        {
            var drivers = repository.Drivers
            .OrderBy(d => d.ID)
            .Skip((page - 1) * listSize)
            .Take(listSize);

            ViewData["listSize"] = listSize;
            ViewData["page"] = page;
            return View(drivers);
        }

        public IActionResult Vehicles(int page = 1)
        {
            var vehicles = repository.Vehicles
            .OrderBy(v => v.ID)
            .Skip((page - 1) * listSize)
            .Take(listSize);

            ViewData["listSize"] = listSize;
            ViewData["page"] = page;
            return View(vehicles);
        }

        public IActionResult Incomings(int page = 1)
        {
            var incomings = repository.Incomings
            .OrderBy(i => i.ID)
            .Skip((page - 1) * listSize)
            .Take(listSize)
            .Include(i => i.Origin)
            .Include(i => i.Product)
            .Include(i => i.Vehicle)
            .Include(i => i.Driver);

            ViewData["listSize"] = listSize;
            ViewData["page"] = page;
            return View(incomings);
        }

        public IActionResult Shippings(int page = 1)
        {
            var shippings = repository.Shippings
            .OrderBy(s => s.ID)
            .Skip((page - 1) * listSize)
            .Take(listSize)
            .Include(s => s.Product)
            .Include(s => s.Destination)
            .Include(s => s.Vehicle)
            .Include(s => s.Driver);

            ViewData["listSize"] = listSize;
            ViewData["page"] = page;
            return View(shippings);
        }

        public IActionResult Enhancements(int page = 1)
        {
            var enhancements = repository.Enhancements
            .OrderBy(e => e.ID)
            .Skip((page - 1) * listSize)
            .Take(listSize)
            .Include(e => e.BaseProduct)
            .Include(e => e.FinalProduct)
            .Include(e => e.Vehicle)
            .Include(e => e.Driver);

            ViewData["listSize"] = listSize;
            ViewData["page"] = page;
            return View(enhancements);
        }
    }
}