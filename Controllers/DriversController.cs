using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WarehouseManager.Models;
using WarehouseManager.Models.ViewModels;
using WarehouseManager.Infrastructure;

namespace WarehouseManager.Controllers
{
    public class DriversController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public DriversController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List([FromQuery]int page = 1)
        {
            IEnumerable<Driver> drivers = null;
            PagingInfo pagingInfo = new PagingInfo(
                repository.Drivers.Count(), itemsPerPage, page);

            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1 ||
                    pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return NotFound();
                }

                drivers = repository.Drivers
                    .OrderBy(d => d.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking();
            }

            ViewData["Title"] = "Lista de Motoristas";
            ViewData["Entity"] = "Motoristas";
            ViewData["Controller"] = "drivers";
            ViewData["Action"] = "list";
            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(drivers),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet]
        public IActionResult Details([FromRoute]int id) =>
            View(repository.Drivers.FirstOrDefault(d => d.ID == id));

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create([FromForm]Driver driver)
        {
            if (ModelState.IsValid)
            {
                repository.Create(driver);
                return RedirectToAction("List");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit([FromRoute]int id) =>
            View(repository.Drivers.FirstOrDefault(c => c.ID == id));

        [HttpPost]
        public IActionResult Edit([FromRoute]int id, [FromForm]Driver driver)
        {
            if (ModelState.IsValid &&
                repository.Drivers.Any(d => d.ID == id))
            {
                driver.ID = id;
                repository.Update(driver);
                return RedirectToAction("List");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Delete([FromRoute]int id) =>
            View(repository.Drivers.FirstOrDefault(d => d.ID == id));

        [HttpPost]
        public IActionResult Delete([FromRoute]int id, [FromForm]Driver driver)
        {
            if (!repository.Drivers.Any(d => d.ID == id))
            {
                return View();
            }

            driver.ID = id;
            repository.Delete(driver);

            return RedirectToAction("List");
        }
    }
}