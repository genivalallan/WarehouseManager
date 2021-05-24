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
    public class EnhancementsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public EnhancementsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List([FromQuery]int page = 1)
        {
            IEnumerable<Enhancement> enhancements = null;
            PagingInfo pagingInfo = new PagingInfo(
                repository.Enhancements.Count(), itemsPerPage, page);
            
            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1 ||
                    pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return NotFound();
                }

                enhancements = repository.Enhancements
                    .OrderBy(e => e.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(e => e.BaseStock.Product)
                    .Include(e => e.FinalStock.Product)
                    .Include(e => e.Driver)
                    .Include(e => e.Vehicle)
                    .AsNoTracking();
            }

            ViewData["Title"] = "Lista de Processamentos";
            ViewData["Entity"] = "Processamentos";
            ViewData["Controller"] = "enhancements";
            ViewData["Action"] = "list";
            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(enhancements),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet]
        public IActionResult Details([FromRoute]int id) =>
            View(repository.Enhancements
                .Include(e => e.BaseStock.Product)
                .Include(e => e.FinalStock.Product)
                .Include(e => e.Driver)
                .Include(e => e.Vehicle)
                .AsNoTracking()
                .FirstOrDefault(e => e.ID == id));

        [HttpGet]
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        [HttpPost]
        public IActionResult Create([FromForm]Enhancement enhancement)
        {
            if (ModelState.IsValid)
            {
                repository.Create(enhancement);
                return RedirectToAction("List");
            }

            PopulateDropDownLists(
                enhancement.BaseStockID,
                enhancement.FinalStockID,
                enhancement.DriverID,
                enhancement.VehicleID
            );
            return View();
        }

        [HttpGet]
        public IActionResult Delete([FromRoute]int id) =>
            View(repository.Enhancements
                .Include(e => e.BaseStock.Product)
                .Include(e => e.FinalStock.Product)
                .Include(e => e.Driver)
                .Include(e => e.Vehicle)
                .FirstOrDefault(e => e.ID == id));

        [HttpPost]
        public IActionResult Delete([FromRoute]int id, [FromForm]Enhancement enhancement)
        {
            if (!repository.Enhancements.Any(e => e.ID == id))
            {
                return View();
            }

            enhancement.ID = id;
            repository.Delete(enhancement);

            return RedirectToAction("List");
        }

        private void PopulateDropDownLists(
            object selectedBaseStock = null,
            object selectedFinalStock = null,
            object selectedDriver = null,
            object selectedVehicle = null
        ){
            var stocks = repository.Stocks
                .Include(s => s.Owner)
                .Include(s => s.Product)
                .OrderBy(s => s.Product.Name)
                .Select(s => new KeyValuePair<int, string>(s.ID, $"{s.Product.Name} - {s.Owner.Name}"));

            var drivers = from d in repository.Drivers orderby d.Name select d;
            var vehicles = from v in repository.Vehicles orderby v.Plate1 select v;

            ViewBag.BaseStocksID = new SelectList(stocks, "Key", "Value", selectedBaseStock);
            ViewBag.FinalStocksID = new SelectList(stocks, "Key", "Value", selectedFinalStock);
            ViewBag.DriversID = new SelectList(drivers.AsNoTracking(), "ID", "Name", selectedDriver);
            ViewBag.VehiclesID = new SelectList(vehicles.AsNoTracking(), "ID", "Plate1", selectedVehicle);
        }
    }
}