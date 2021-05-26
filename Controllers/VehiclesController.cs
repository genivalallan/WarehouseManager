using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WarehouseManager.Models;
using WarehouseManager.Models.ViewModels;
using WarehouseManager.Infrastructure;

namespace WarehouseManager.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public VehiclesController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List([FromQuery]int page = 1)
        {
            IEnumerable<Vehicle> vehicles = null;
            PagingInfo pagingInfo = new PagingInfo(
                repository.Vehicles.Count(), itemsPerPage, page);
            
            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1 ||
                    pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return NotFound();
                }

                vehicles = repository.Vehicles
                    .OrderBy(v => v.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking();
            }

            ViewData["Title"] = "Lista de Veículos";
            ViewData["Entity"] = "Veículos";
            ViewData["Controller"] = "vehicles";
            ViewData["Action"] = "list";
            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(vehicles),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet]
        public IActionResult Details([FromRoute]int id) =>
            View(repository.Vehicles.FirstOrDefault(v => v.ID == id));

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create([FromForm]Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                repository.Create(vehicle);
                return RedirectToAction("List");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit([FromRoute]int id) =>
            View(repository.Vehicles.FirstOrDefault(c => c.ID == id));

        [HttpPost]
        public IActionResult Edit([FromRoute]int id, [FromForm]Vehicle vehicle)
        {
            if (!ModelState.IsValid &&
                !repository.Vehicles.Any(v => v.ID == id))
            {
                return View(vehicle);
            }

            vehicle.ID = id;
            repository.Update(vehicle);
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Delete([FromRoute]int id) =>
            View(repository.Vehicles.FirstOrDefault(v => v.ID == id));

        [HttpPost]
        public IActionResult Delete(int id, Vehicle vehicle)
        {
            if (!repository.Vehicles.Any(v => v.ID == id))
            {
                return View();
            }

            vehicle.ID = id;
            repository.Delete(vehicle);

            return RedirectToAction("List");
        }
    }
}