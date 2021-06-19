using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

using WarehouseManager.Models;
using WarehouseManager.Models.ViewModels;
using WarehouseManager.Infrastructure;

namespace WarehouseManager.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IWMRepository repository;

        public ClientsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List(
            string orderby,
            string order,
            string searchby,
            string search,
            int page = 1
        ){
            List<Client> clients = null;
            PagingInfo paging = new PagingInfo();
            ListFilter filter = new ListFilter();
            MySqlParameter p0 = null;
            string queryCondition = "";

            orderby = String.IsNullOrEmpty(orderby) ? "id" : orderby.Trim();
            order = String.IsNullOrEmpty(order) ? "asc" : order.Trim();

            if (repository.Clients.Count() != 0)
            {
                if ((orderby != "id" && orderby != "name" && orderby != "address") ||
                    (order != "asc" && order != "desc"))
                {
                    return RedirectToAction();
                }

                if (!String.IsNullOrEmpty(search = search?.Trim()))
                {
                    searchby = searchby?.Trim();
                    if (searchby != "name" && searchby != "address")
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

                string filterQuery = "SELECT * FROM client";
                filterQuery += queryCondition;
                filterQuery += $" ORDER BY {filter.OrderBy} {filter.Order}";

                paging.TotalItems = repository.DbContext().Clients
                    .FromSqlRaw(filterQuery, p0).Count();
                
                paging.Page = (page <= 1) ? 1 : (page >= paging.TotalPages ? paging.TotalPages : page);
                
                clients = repository.DbContext().Clients
                    .FromSqlRaw(filterQuery, p0)
                    .Skip((paging.Page - 1) * paging.ItemsPerPage)
                    .Take(paging.ItemsPerPage)
                    .AsNoTracking()
                    .ToList();
            }

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(clients),
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

            object query = new {
                page = (page == 1) ? null : page.ToString(),
                orderby = (orderby == "id") ? null : orderby.Trim(),
                order = (order == "asc") ? null : order.Trim(),
                searchby = String.IsNullOrEmpty(search) ? null : searchby.Trim(),
                search
            };

            return RedirectToAction("List", "Clients", query);
        }

        [HttpGet]
        public IActionResult Details([FromRoute]int id) =>
            View(repository.Clients.FirstOrDefault(c => c.ID == id));

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create([FromForm]Client client)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            repository.Create(client);

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Edit([FromRoute]int id) =>
            View(repository.Clients.FirstOrDefault(c => c.ID == id));

        [HttpPost]
        public IActionResult Edit([FromRoute]int id, [FromForm]Client client)
        {
            if (!ModelState.IsValid ||
                !repository.Clients.Any(c => c.ID == id))
            {
                return View(client);
            }

            client.ID = id;
            repository.Update(client);

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Delete([FromRoute]int id) =>
            View(repository.Clients.FirstOrDefault(c => c.ID == id));

        [HttpPost]
        public IActionResult Delete([FromRoute]int id, [FromForm]Client client)
        {
            if (!repository.Clients.Any(c => c.ID == id))
            {
                return View();
            }

            client.ID = id;
            repository.Delete(client);

            return RedirectToAction("List");
        }
    }
}