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
        public IActionResult List()
        {
            IEnumerable<Driver> drivers = null;
            PagingInfo pagingInfo = new PagingInfo()
                .Create(repository.Drivers.Count(), itemsPerPage, HttpContext.Request.Query["page"]);

            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page< 1)
                {
                    return Redirect("/drivers");
                }
                else if (pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return Redirect($"/drivers?page={pagingInfo.TotalPages}");
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
        public IActionResult Details(int id) =>
            View(repository.Drivers.FirstOrDefault(d => d.ID == id));

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Driver driver)
        {
            if (ModelState.IsValid)
            {
                repository.Add(driver);
                return RedirectToAction("List");
            }

            return View();
        }
    }
}