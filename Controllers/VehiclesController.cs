using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WarehouseManager.Models;
using WarehouseManager.Models.ViewModels;

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
            int page;
            if (HttpContext.Request.Query["page"] == "" ||
                !Int32.TryParse(HttpContext.Request.Query["page"], out page))
            {
                page = 1;
            }

            IEnumerable<Vehicle> vehicles = null;
            PagingInfo pagingInfo = null;
            if (repository.Vehicles.Count() != 0)
            {
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = itemsPerPage,
                    TotalItems = repository.Vehicles.Count()
                };

                if (page < 1)
                {
                    return Redirect("/vehicles");
                }
                else if (page > pagingInfo.TotalPages)
                {
                    return Redirect($"/vehicles?page={pagingInfo.TotalPages}");
                }
                else
                {
                    pagingInfo.Page = page;
                }

                vehicles = repository.Vehicles
                    .OrderBy(v => v.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking();
            }

            return View(new ListViewModel<Vehicle>
            {
                Items = vehicles,
                PagingInfo = pagingInfo
            });
        }
    }
}