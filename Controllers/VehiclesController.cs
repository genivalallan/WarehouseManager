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
    [ApiController]
    [Route("[controller]")]
    public class VehiclesController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public VehiclesController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetVehicles()
        {
            IEnumerable<Vehicle> vehicles = null;
            PagingInfo pagingInfo = new PagingInfo()
                .Create(repository.Vehicles.Count(), itemsPerPage, HttpContext.Request.Query["page"]);
            
            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1)
                {
                    return Redirect("/vehicles");
                }
                else if (pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return Redirect($"/vehicles?page={pagingInfo.TotalPages}");
                }

                vehicles = repository.Vehicles
                    .OrderBy(v => v.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking();
            }

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(vehicles),
                PagingInfo = pagingInfo
            });
        }
    }
}