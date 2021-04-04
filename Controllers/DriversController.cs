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
    [ApiController]
    [Route("[controller]")]
    public class DriversController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public DriversController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetDrivers()
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

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(drivers),
                PagingInfo = pagingInfo
            });
        }
    }
}