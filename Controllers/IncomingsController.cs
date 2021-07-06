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
    public class IncomingsController : Controller
    {
        private readonly IWMRepository repository;

        public IncomingsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List(
            string orderby,
            string order,
            string searchby,
            string search,
            string initDate,
            string endDate,
            int page = 1
        ){
            List<Incoming> incomings = null;
            PagingInfo paging = new PagingInfo();
            ListFilter filter = new ListFilter();

            if (repository.Incomings.Count() != 0)
            {
                orderby = String.IsNullOrWhiteSpace(orderby) ? "id" : orderby.Trim();
                order = String.IsNullOrWhiteSpace(order) ? "asc" : order.Trim();

                if ((orderby != "id" && orderby != "origin" && orderby != "dest" && orderby != "date") ||
                    (order != "asc" && order != "desc"))
                {
                    return RedirectToAction();
                }

                bool hasInitDate = false;
                bool hasEndDate = false;
                DateTime idt = new DateTime();
                DateTime edt = new DateTime();
                if (!String.IsNullOrWhiteSpace(initDate))
                {
                    if (!DateTime.TryParse(initDate, out idt))
                    {
                        return RedirectToAction();
                    }
                    hasInitDate = true;
                }
                if (!String.IsNullOrWhiteSpace(endDate))
                {
                    if (!DateTime.TryParse(endDate, out edt))
                    {
                        return RedirectToAction();
                    }
                    hasEndDate = true;
                }
                if (hasInitDate && hasEndDate && idt > edt)
                {
                    return RedirectToAction();
                }

                filter.Order = order;
                filter.OrderBy = orderby;

                string filterQuery = "SELECT incoming.*, dest, client.name AS origin FROM incoming"
                    + " INNER JOIN (SELECT stock.id AS s_id, CONCAT(product.name, ' - ', client.name) AS dest FROM stock"
                    + " INNER JOIN product ON stock.product_id = product.id"
                    + " INNER JOIN client ON stock.client_id = client.id) AS t1"
                    + " ON incoming.stock_id = t1.s_id"
                    + " INNER JOIN client ON incoming.client_id = client.id";

                MySqlParameter p0 = null;
                MySqlParameter p1 = null;
                MySqlParameter p2 = null;
                if (!String.IsNullOrWhiteSpace(search = search?.Trim()))
                {
                    searchby = searchby?.Trim();
                    if (searchby != "origin" && searchby != "dest")
                    {
                        return RedirectToAction();
                    }
                    else
                    {
                        filter.SearchBy = searchby;
                        filter.Search = search;
                        filterQuery += $" WHERE {(searchby == "origin" ? "client.name" : "dest")} LIKE @search";
                        p0 = new MySqlParameter("@search", $"%{search}%");
                    }
                }

                if (hasInitDate)
                {
                    filterQuery += $" {(filterQuery.Contains("WHERE") ? "AND" : "WHERE")} CAST(incoming.created_at AS DATE) >= CAST(@initdate AS DATE)";
                    p1 = new MySqlParameter("@initdate", initDate);
                }
                if (hasEndDate)
                {
                    filterQuery += $" {(filterQuery.Contains("WHERE") ? "AND" : "WHERE")} CAST(incoming.created_at AS DATE) <= CAST(@enddate AS DATE)";
                    p2 = new MySqlParameter("@enddate", endDate);
                }
                filterQuery += $" ORDER BY {(filter.OrderBy == "date" ? "created_at" : filter.OrderBy)} {filter.Order}";

                paging.TotalItems = repository.DbContext().Incomings
                    .FromSqlRaw(filterQuery, p0, p1, p2).Count();

                paging.Page = (page <= 1) ? 1 : (page >= paging.TotalPages ? paging.TotalPages : page);

                incomings = repository.DbContext().Incomings
                    .FromSqlRaw(filterQuery, p0, p1, p2)
                    .Skip((paging.Page - 1) * paging.ItemsPerPage)
                    .Take(paging.ItemsPerPage)
                    .Include(i => i.Client)
                    .Include(i => i.Stock).ThenInclude(s => s.Product)
                    .Include(i => i.Stock).ThenInclude(s => s.Owner)
                    .Include(i => i.Driver)
                    .Include(i => i.Vehicle)
                    .AsNoTracking().ToList();
            }

            ViewData["initDate"] = initDate;
            ViewData["endDate"] = endDate;
            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(incomings),
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

            return RedirectToAction("List", "Incomings", query);
        }

        [HttpGet]
        public IActionResult Details([FromRoute]int id) =>
            View(repository.Incomings
                .Include(i => i.Client)
                .Include(i => i.Stock.Product)
                .Include(i => i.Driver)
                .Include(i => i.Vehicle)
                .AsNoTracking()
                .FirstOrDefault(i => i.ID == id));

        [HttpGet]
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        [HttpPost]
        public IActionResult Create([FromForm]Incoming incoming)
        {
            if (ModelState.IsValid)
            {
                repository.Create(incoming);
                return RedirectToAction("List");
            }

            PopulateDropDownLists(
                incoming.ClientID,
                incoming.StockID,
                incoming.DriverID,
                incoming.VehicleID
            );
            return View();
        }

        [HttpGet]
        public IActionResult Delete([FromRoute]int id) =>
            View(repository.Incomings
                .Include(i => i.Client)
                .Include(i => i.Stock.Product)
                .Include(i => i.Driver)
                .Include(i => i.Vehicle)
                .FirstOrDefault(i => i.ID == id));

        [HttpPost]
        public IActionResult Delete([FromRoute]int id, [FromForm]Incoming incoming)
        {
            if (!repository.Incomings.Any(i => i.ID == id))
            {
                return View();
            }

            incoming.ID = id;
            repository.Delete(incoming);

            return RedirectToAction("List");
        }

        private void PopulateDropDownLists(
            object selectedClient = null,
            object selectedStock = null,
            object selectedDriver = null,
            object selectedVehicle = null
        )
        {
            var stocks = repository.Stocks
                .Include(s => s.Owner)
                .Include(s => s.Product)
                .OrderBy(s => s.Product.Name)
                .Select(s => new KeyValuePair<int, string>(s.ID, $"{s.ID} - {s.Product.Name} - {s.Owner.Name}"));
            var clients = from c in repository.Clients orderby c.Name select c;
            var drivers = from d in repository.Drivers orderby d.Name select d;
            var vehicles = from v in repository.Vehicles orderby v.Plate1 select v;

            ViewBag.ClientsID = new SelectList(clients, "ID", "Name", selectedClient);
            ViewBag.StocksID = new SelectList(stocks, "Key", "Value", selectedStock);
            ViewBag.DriversID = new SelectList(drivers.AsNoTracking(), "ID", "Name", selectedDriver);
            ViewBag.VehiclesID = new SelectList(vehicles.AsNoTracking(), "ID", "Plate1", selectedVehicle);
        }
    }
}