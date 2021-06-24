using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

using WarehouseManager.Models;
using WarehouseManager.Models.ViewModels;
using WarehouseManager.Infrastructure;

namespace WarehouseManager.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly IWMRepository repository;

        public VehiclesController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List(
            string orderby,
            string order,
            string searchby,
            string search,
            int page = 1
        ){
            List<Vehicle> vehicles = null;
            PagingInfo paging = new PagingInfo();
            ListFilter filter = new ListFilter();
            MySqlParameter p0 = null;
            string queryCondition = "";

            orderby = String.IsNullOrEmpty(orderby) ? "id" : orderby.Trim();
            order = String.IsNullOrEmpty(order) ? "asc" : order.Trim();

            if (repository.Vehicles.Count() != 0)
            {
                if ((orderby != "id" && orderby != "plate1" && orderby != "plate2" && orderby != "plate3" && orderby != "rntrc") ||
                    (order != "asc" && order != "desc"))
                {
                    return RedirectToAction();
                }

                if (!String.IsNullOrEmpty(search = search?.Trim()))
                {
                    searchby = searchby?.Trim();
                    if (searchby != "plate1" && searchby != "plate2" && searchby != "plate3" && searchby != "rntrc")
                    {
                        return RedirectToAction();
                    }
                    else
                    {
                        filter.SearchBy = searchby;
                        filter.Search = search;
                        queryCondition = $" WHERE {searchby} LIKE @search";
                        p0 = new MySqlParameter("@search", $"%{search}%");
                    }
                }

                filter.Order = order;
                filter.OrderBy = orderby;

                string filterQuery = "SELECT * FROM vehicle";
                filterQuery += queryCondition;
                filterQuery += $" ORDER BY {filter.OrderBy} {filter.Order}";

                paging.TotalItems = repository.DbContext().Vehicles
                    .FromSqlRaw(filterQuery, p0).Count();

                paging.Page = (page <= 1) ? 1 : (page >= paging.TotalPages ? paging.TotalPages : page);

                vehicles = repository.DbContext().Vehicles
                    .FromSqlRaw(filterQuery, p0)
                    .Skip((paging.Page - 1) * paging.ItemsPerPage)
                    .Take(paging.ItemsPerPage)
                    .AsNoTracking()
                    .ToList();
            }

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(vehicles),
                PagingInfo = paging,
                ListFilter = filter
            });
        }

        [HttpPost]
        public IActionResult List(
            [FromForm]string orderby,
            [FromForm]string order,
            [FromForm]string searchby,
            [FromForm]string search,
            int _,
            [FromForm]int page = 1
        ){
            orderby ??= "";
            order ??= "";
            searchby ??= "";
            search = search?.Trim();

            object query = new
            {
                page = (page == 1) ? null : page.ToString(),
                orderby = (orderby == "id") ? null : orderby.Trim(),
                order = (order == "asc") ? null : order.Trim(),
                searchby = String.IsNullOrEmpty(search) ? null : searchby,
                search
            };

            return RedirectToAction("List", "Vehicles", query);
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