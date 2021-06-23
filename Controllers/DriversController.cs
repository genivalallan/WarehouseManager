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
    public class DriversController : Controller
    {
        private readonly IWMRepository repository;

        public DriversController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List(
            string orderby,
            string order,
            string searchby,
            string search,
            int page = 1
        ){
            List<Driver> drivers = null;
            PagingInfo paging = new PagingInfo();
            ListFilter filter = new ListFilter();
            MySqlParameter p0 = null;
            string queryCondition = "";

            orderby = String.IsNullOrEmpty(orderby) ? "id" : orderby.Trim();
            order = String.IsNullOrEmpty(order) ? "asc" : order.Trim();

            if (repository.Drivers.Count() != 0)
            {
                if ((orderby != "id" && orderby != "name" && orderby != "cnh") ||
                    (order != "asc" && order != "desc"))
                {
                    return RedirectToAction();
                }

                if (!String.IsNullOrEmpty(search = search?.Trim()))
                {
                    searchby = searchby?.Trim();
                    if (searchby != "name" && searchby != "cnh")
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

                string filterQuery = "SELECT * FROM driver";
                filterQuery += queryCondition;
                filterQuery += $" ORDER BY {filter.OrderBy} {filter.Order}";

                paging.TotalItems = repository.DbContext().Drivers
                    .FromSqlRaw(filterQuery, p0).Count();

                paging.Page = (page <= 1) ? 1 : (page >= paging.TotalPages ? paging.TotalPages : page);

                drivers = repository.DbContext().Drivers
                    .FromSqlRaw(filterQuery, p0)
                    .Skip((paging.Page - 1) * paging.ItemsPerPage)
                    .Take(paging.ItemsPerPage)
                    .AsNoTracking()
                    .ToList();
            }

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(drivers),
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
        )
        {
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

            return RedirectToAction("List", "Drivers", query);
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
            if (!ModelState.IsValid &&
                !repository.Drivers.Any(d => d.ID == id))
            {
                return View(driver);
            }

            driver.ID = id;
            repository.Update(driver);
            return RedirectToAction("List");
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