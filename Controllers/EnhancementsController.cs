using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

using WarehouseManager.Models;
using WarehouseManager.Models.ViewModels;
using WarehouseManager.Infrastructure;

namespace WarehouseManager.Controllers
{
    public class EnhancementsController : Controller
    {
        private readonly IWMRepository repository;

        public EnhancementsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List(
            string orderby,
            string order,
            string searchby,
            string search,
            string initdate,
            string enddate,
            int page = 1
        ){
            List<Enhancement> enhancements = null;
            PagingInfo paging = new PagingInfo();
            ListFilter filter = new ListFilter();

            if (repository.Enhancements.Count() != 0)
            {
                orderby = String.IsNullOrWhiteSpace(orderby) ? "id" : orderby.Trim();
                order = String.IsNullOrWhiteSpace(order) ? "asc" : order.Trim();

                if ((orderby != "id" && orderby != "base" && orderby != "final" && orderby != "date") ||
                    (order != "asc" && order != "desc"))
                {
                    return RedirectToAction();
                }

                bool hasInitDate = false;
                bool hasEndDate = false;
                DateTime idt = new DateTime();
                DateTime edt = new DateTime();
                if (!String.IsNullOrWhiteSpace(initdate))
                {
                    if (!DateTime.TryParse(initdate, out idt))
                    {
                        return RedirectToAction();
                    }
                    hasInitDate = true;
                    filter.InitDate = initdate;
                }
                if (!String.IsNullOrWhiteSpace(enddate))
                {
                    if (!DateTime.TryParse(enddate, out edt))
                    {
                        return RedirectToAction();
                    }
                    hasEndDate = true;
                    filter.EndDate = enddate;
                }
                if (hasInitDate && hasEndDate && idt > edt)
                {
                    return RedirectToAction();
                }

                filter.Order = order;
                filter.OrderBy = orderby;

                string filterQuery = "SELECT enhancement.*, base, final FROM enhancement"
                    + " INNER JOIN (SELECT stock.id AS s1_id, CONCAT(product.name, ' - ', client.name) AS base FROM stock"
                    + " INNER JOIN product ON stock.product_id = product.id"
                    + " INNER JOIN client ON stock.client_id = client.id) AS t1"
                    + " ON enhancement.base_stock_id = t1.s1_id"
                    + " INNER JOIN (SELECT stock.id AS s2_id, CONCAT(product.name, ' - ', client.name) AS final FROM stock"
                    + " INNER JOIN product ON stock.product_id = product.id"
                    + " INNER JOIN client ON stock.client_id = client.id) AS t2"
                    + " ON enhancement.final_stock_id = t2.s2_id";

                MySqlParameter p0 = null;
                MySqlParameter p1 = null;
                MySqlParameter p2 = null;
                if (!String.IsNullOrWhiteSpace(search = search?.Trim()))
                {
                    searchby = searchby?.Trim();
                    if (searchby != "base" && searchby != "final")
                    {
                        return RedirectToAction();
                    }
                    else
                    {
                        filter.SearchBy = searchby;
                        filter.Search = search;
                        filterQuery += $" WHERE {searchby} LIKE @search";
                        p0 = new MySqlParameter("@search", $"%{search}%");
                    }
                }

                if (hasInitDate)
                {
                    filterQuery += $" {(filterQuery.Contains("WHERE") ? "AND" : "WHERE")} CAST(enhancement.created_at AS DATE) >= CAST(@initdate AS DATE)";
                    p1 = new MySqlParameter("@initdate", initdate);
                }
                if (hasEndDate)
                {
                    filterQuery += $" {(filterQuery.Contains("WHERE") ? "AND" : "WHERE")} CAST(enhancement.created_at AS DATE) <= CAST(@enddate AS DATE)";
                    p2 = new MySqlParameter("@enddate", enddate);
                }
                filterQuery += $" ORDER BY {(filter.OrderBy == "date" ? "created_at" : filter.OrderBy)} {filter.Order}";

                paging.TotalItems = repository.DbContext().Enhancements
                    .FromSqlRaw(filterQuery, p0, p1, p2).Count();

                paging.Page = (page <= 1) ? 1 : (page >= paging.TotalPages ? paging.TotalPages : page);

                enhancements = repository.DbContext().Enhancements
                    .FromSqlRaw(filterQuery, p0, p1, p2)
                    .Skip((paging.Page - 1) * paging.ItemsPerPage)
                    .Take(paging.ItemsPerPage)
                    .Include(e => e.BaseStock).ThenInclude(s => s.Product)
                    .Include(e => e.BaseStock).ThenInclude(s => s.Owner)
                    .Include(e => e.FinalStock).ThenInclude(s => s.Product)
                    .Include(e => e.FinalStock).ThenInclude(s => s.Owner)
                    .Include(e => e.Driver)
                    .Include(e => e.Vehicle)
                    .AsNoTracking().ToList();
            }

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(enhancements),
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
            [FromForm]string initDate,
            [FromForm]string endDate,
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
                search,
                initDate = DateTime.TryParse(initDate, out DateTime _) ? initDate : null,
                endDate = DateTime.TryParse(endDate, out DateTime _) ? endDate : null
            };

            return RedirectToAction("List", "Enhancements", query);
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
                .Select(s => new KeyValuePair<int, string>(s.ID, $"{s.ID} - {s.Product.Name} - {s.Owner.Name}"));

            var drivers = from d in repository.Drivers orderby d.Name select d;
            var vehicles = from v in repository.Vehicles orderby v.Plate1 select v;

            ViewBag.BaseStocksID = new SelectList(stocks, "Key", "Value", selectedBaseStock);
            ViewBag.FinalStocksID = new SelectList(stocks, "Key", "Value", selectedFinalStock);
            ViewBag.DriversID = new SelectList(drivers.AsNoTracking(), "ID", "Name", selectedDriver);
            ViewBag.VehiclesID = new SelectList(vehicles.AsNoTracking(), "ID", "Plate1", selectedVehicle);
        }
    }
}