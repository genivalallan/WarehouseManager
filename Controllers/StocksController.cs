using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

using WarehouseManager.Models;
using WarehouseManager.Models.ViewModels;
using WarehouseManager.Infrastructure;

namespace WarehouseManager.Controllers
{
    public class StocksController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public StocksController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List()
        {
            IEnumerable<Stock> stocks = null;
            PagingInfo pagingInfo = new PagingInfo()
                .Create(repository.Stocks.Count(), itemsPerPage, HttpContext.Request.Query["page"]);
            
            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1)
                {
                    return Redirect("/stocks");
                }
                else if (pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return Redirect($"/stocks?page={pagingInfo.TotalPages}");
                }

                stocks = repository.Stocks
                    .OrderBy(s => s.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(s => s.Product)
                    .Include(s => s.Owner)
                    .AsNoTracking();
            }

            ViewData["Title"] = "Lista de Estoques";
            ViewData["Entity"] = "Estoques";
            ViewData["Controller"] = "stocks";
            ViewData["Action"] = "list";
            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(stocks),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet]
        public IActionResult Details(int id) =>
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
        public IActionResult Create(Stock stock)
        {
            if (ModelState.IsValid)
            {
                repository.Add(stock);
                return RedirectToAction("List");
            }

            PopulateDropDownLists(stock.ClientID, stock.ProductID);
            return View();
        }

        [HttpGet]
        public IActionResult Delete(int id) =>
            View(repository.Stocks
                .Include(s => s.Owner)
                .Include(s => s.Product)
                .FirstOrDefault(s => s.ID == id));

        [HttpPost]
        public IActionResult Delete(int id, Stock stock)
        {
            if (!repository.Stocks.Any(s => s.ID == id))
            {
                return View();
            }

            stock.ID = id;
            repository.Delete(stock);

            return RedirectToAction("List");
        }

        private void PopulateDropDownLists(object selectdClient = null, object selectedProduct = null)
        {
            var clients = from c in repository.Clients orderby c.Name select c;
            var products = from p in repository.Products orderby p.Name select p;

            ViewBag.ClientsID = new SelectList(clients.AsNoTracking(), "ID", "Name", selectdClient);
            ViewBag.ProductsID = new SelectList(products.AsNoTracking(), "ID", "Name", selectedProduct);
        }
    }
}