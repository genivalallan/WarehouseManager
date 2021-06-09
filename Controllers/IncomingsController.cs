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
    public class IncomingsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public IncomingsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List([FromQuery]int page = 1)
        {
            IEnumerable<Incoming> incomings = null;
            PagingInfo pagingInfo = new PagingInfo(
                repository.Incomings.Count(), itemsPerPage, page);

            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1 ||
                    pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return NotFound();
                }

                incomings = repository.Incomings
                    .OrderBy(i => i.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(i => i.Client)
                    .Include(i => i.Stock.Product)
                    .Include(i => i.Stock.Owner)
                    .Include(i => i.Driver)
                    .Include(i => i.Vehicle)
                    .AsNoTracking();
            }

            ViewData["Title"] = "Lista de Entradas";
            ViewData["Entity"] = "Entradas";
            ViewData["Controller"] = "incomings";
            ViewData["Action"] = "list";
            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(incomings),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet]
        public IActionResult Details([FromRoute]int id) =>
            View(repository.Incomings
                .Include(i => i.Client)
                .Include(i => i.Stock.Product)
                .Include(i => i.Driver)
                .Include(i => i.Vehicle)
                .AsNoTracking()
                .FirstOrDefault(i => i.ID == id));

        [HttpGet]
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        [HttpPost]
        public IActionResult Create([FromForm]Incoming incoming)
        {
            if (ModelState.IsValid)
            {
                repository.Create(incoming);
                return RedirectToAction("List");
            }

            PopulateDropDownLists(
                incoming.ClientID,
                incoming.StockID,
                incoming.DriverID,
                incoming.VehicleID
            );
            return View();
        }

        [HttpGet]
        public IActionResult Delete([FromRoute]int id) =>
            View(repository.Incomings
                .Include(i => i.Client)
                .Include(i => i.Stock.Product)
                .Include(i => i.Driver)
                .Include(i => i.Vehicle)
                .FirstOrDefault(i => i.ID == id));

        [HttpPost]
        public IActionResult Delete([FromRoute]int id, [FromForm]Incoming incoming)
        {
            if (!repository.Incomings.Any(i => i.ID == id))
            {
                return View();
            }

            incoming.ID = id;
            repository.Delete(incoming);

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
                .Select(s => new KeyValuePair<int, string>(s.ID, $"{s.ID} - {s.Product.Name} - {s.Owner.Name}"));
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