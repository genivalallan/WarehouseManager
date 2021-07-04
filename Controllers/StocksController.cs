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
    public class StocksController : Controller
    {
        private readonly IWMRepository repository;

        public StocksController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List(
            string orderby,
            string order,
            string searchby,
            string search,
            int page = 1
        ){
            List<Stock> stocks = null;
            PagingInfo paging = new PagingInfo();
            ListFilter filter = new ListFilter();

            if (repository.Stocks.Count() != 0)
            {
                orderby = String.IsNullOrWhiteSpace(orderby) ? "id" : orderby.Trim();
                order = String.IsNullOrWhiteSpace(order) ? "asc" : order.Trim();

                if ((orderby != "id" && orderby != "product" && orderby != "owner" && orderby != "balance") ||
                    (order != "asc" && order != "desc"))
                {
                    return RedirectToAction();
                }

                filter.Order = order;
                filter.OrderBy = orderby;

                string filterQuery = "SELECT stock.*, product.name AS product, client.name AS owner FROM stock"
                    + " INNER JOIN product ON stock.product_id = product.id"
                    + " INNER JOIN client ON stock.client_id = client.id";

                MySqlParameter p0 = null;
                if (!String.IsNullOrWhiteSpace(search = search?.Trim()))
                {
                    searchby = searchby?.Trim();
                    if (searchby != "product" && searchby != "owner")
                    {
                        return RedirectToAction();
                    }
                    else
                    {
                        filter.SearchBy = searchby;
                        filter.Search = search;
                        filterQuery += $" WHERE {(searchby == "owner" ? "client" : "product")}.name LIKE @search";
                        p0 = new MySqlParameter("@search", $"%{filter.Search}%");
                    }
                }

                filterQuery += $" ORDER BY {orderby} {filter.Order}";

                paging.TotalItems = repository.DbContext().Stocks
                    .FromSqlRaw(filterQuery, p0).Count();

                paging.Page = (page <= 1) ? 1 : (page >= paging.TotalPages ? paging.TotalPages : page);

                stocks = repository.DbContext().Stocks
                    .FromSqlRaw(filterQuery, p0)
                    .Skip((paging.Page - 1) * paging.ItemsPerPage)
                    .Take(paging.ItemsPerPage)
                    .AsNoTracking()
                    .ToList();

                for (int i = 0; i < stocks.Count; i++)
                {
                    stocks[i].Product = repository.Products.Single(p => p.ID == stocks[i].ProductID);
                    stocks[i].Owner = repository.Clients.Single(p => p.ID == stocks[i].ClientID);
                }
            }

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(stocks),
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

            return RedirectToAction("List", "Stocks", query);
        }

        [HttpGet]
        public IActionResult Details([FromRoute]int id) =>
            View(repository.Stocks
                .Include(s => s.Owner)
                .Include(s => s.Product)
                .AsNoTracking()
                .FirstOrDefault(s => s.ID == id));

        [HttpGet]
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        [HttpPost]
        public IActionResult Create([FromForm]Stock stock)
        {
            if (ModelState.IsValid)
            {
                repository.Create(stock);
                return RedirectToAction("List");
            }

            PopulateDropDownLists(stock.ClientID, stock.ProductID);
            return View();
        }

        [HttpGet]
        public IActionResult Delete([FromRoute]int id) =>
            View(repository.Stocks
                .Include(s => s.Owner)
                .Include(s => s.Product)
                .FirstOrDefault(s => s.ID == id));

        [HttpPost]
        public IActionResult Delete([FromRoute]int id, [FromForm]Stock stock)
        {
            if (!repository.Stocks.Any(s => s.ID == id))
            {
                return View();
            }

            stock.ID = id;
            repository.Delete(stock);

            return RedirectToAction("List");
        }

        private void PopulateDropDownLists(
            object selectdClient = null,
            object selectedProduct = null
        ){
            var clients = from c in repository.Clients orderby c.Name select c;
            var products = from p in repository.Products orderby p.Name select p;

            ViewBag.ClientsID = new SelectList(clients.AsNoTracking(), "ID", "Name", selectdClient);
            ViewBag.ProductsID = new SelectList(products.AsNoTracking(), "ID", "Name", selectedProduct);
        }
    }
}