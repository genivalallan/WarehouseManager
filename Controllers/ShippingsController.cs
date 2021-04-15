using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

using WarehouseManager.Models;
using WarehouseManager.Models.ViewModels;
using WarehouseManager.Infrastructure;

namespace WarehouseManager.Controllers
{
    public class ShippingsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public ShippingsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List()
        {
            IEnumerable<Shipping> shippings = null;
            PagingInfo pagingInfo = new PagingInfo(
                repository.Shippings.Count(), itemsPerPage, HttpContext.Request.Query["page"]);
            
            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1 ||
                    pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return NotFound();
                }

                shippings = repository.Shippings
                    .OrderBy(s => s.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(s => s.Stock.Product)
                    .Include(s => s.Client)
                    .Include(s => s.Driver)
                    .Include(s => s.Vehicle)
                    .AsNoTracking();
            }

            ViewData["Title"] = "Lista de Saídas";
            ViewData["Entity"] = "Saídas";
            ViewData["Controller"] = "shippings";
            ViewData["Action"] = "list";
            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(shippings),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet]
        public IActionResult Details(int id) =>
            View(repository.Shippings
                .Include(s => s.Client)
                .Include(s => s.Stock.Product)
                .Include(s => s.Driver)
                .Include(s => s.Vehicle)
                .AsNoTracking()
                .FirstOrDefault(s => s.ID == id));

        [HttpGet]
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Shipping shipping)
        {
            if (ModelState.IsValid)
            {
                repository.Create(shipping);
                return RedirectToAction("List");
            }

            PopulateDropDownLists(
                shipping.ClientID,
                shipping.StockID,
                shipping.DriverID,
                shipping.VehicleID
            );
            return View();
        }

        [HttpGet]
        public IActionResult Delete(int id) =>
            View(repository.Shippings
                .Include(s => s.Client)
                .Include(s => s.Stock.Product)
                .Include(s => s.Driver)
                .Include(s => s.Vehicle)
                .FirstOrDefault(s => s.ID == id));

        [HttpPost]
        public IActionResult Delete(int id, Shipping shipping)
        {
            if (!repository.Shippings.Any(s => s.ID == id))
            {
                return View();
            }

            shipping.ID = id;
            repository.Delete(shipping);

            return RedirectToAction("List");
        }

        private void PopulateDropDownLists(
            object selectedClient = null,
            object selectedStock = null,
            object selectedDriver = null,
            object selectedVehicle = null
        )
        {
            var stocks = repository.Stocks
                .Include(s => s.Owner)
                .Include(s => s.Product)
                .OrderBy(s => s.Product.Name)
                .Select(s => new KeyValuePair<int, string>(s.ID, $"{s.Product.Name} - {s.Owner.Name}"));
            var clients = from c in repository.Clients orderby c.Name select c;
            var drivers = from d in repository.Drivers orderby d.Name select d;
            var vehicles = from v in repository.Vehicles orderby v.Plate1 select v;

            ViewBag.ClientsID = new SelectList(clients, "ID", "Name", selectedClient);
            ViewBag.StocksID = new SelectList(stocks, "Key", "Value", selectedStock);
            ViewBag.DriversID = new SelectList(drivers.AsNoTracking(), "ID", "Name", selectedDriver);
            ViewBag.VehiclesID = new SelectList(vehicles.AsNoTracking(), "ID", "Plate1", selectedVehicle);
        }
    }
}