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
    public class DriversController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public DriversController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetDrivers()
        {
            int page;
            if (HttpContext.Request.Query["page"] == "" ||
                !Int32.TryParse(HttpContext.Request.Query["page"], out page))
            {
                page = 1;
            }

            IEnumerable<Driver> drivers = null;
            PagingInfo pagingInfo = null;
            if (repository.Drivers.Count() != 0)
            {
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = itemsPerPage,
                    TotalItems = repository.Drivers.Count()
                };

                if (page < 1)
                {
                    return Redirect("/drivers");
                }
                else if (page > pagingInfo.TotalPages)
                {
                    return Redirect($"/drivers?page={pagingInfo.TotalPages}");
                }
                else
                {
                    pagingInfo.Page = page;
                }

                drivers = repository.Drivers
                    .OrderBy(d => d.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking();
            }

            return View(new ListViewModel<Driver>
            {
                Items = drivers,
                PagingInfo = pagingInfo
            });
        }
    }
}