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
    public class ProductsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public ProductsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult List([FromQuery]int page = 1)
        {
            IEnumerable<Product> products = null;
            PagingInfo pagingInfo = new PagingInfo(
                repository.Products.Count(), itemsPerPage, page);
            
            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1 ||
                    pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return NotFound();
                }

                products = repository.Products
                    .OrderBy(p => p.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking();
            }

            ViewData["Title"] = "Lista de Produtos";
            ViewData["Entity"] = "Produtos";
            ViewData["Controller"] = "products";
            ViewData["Action"] = "list";
            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(products),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet]
        public IActionResult Details([FromRoute]int id) =>
            View(repository.Products.FirstOrDefault(p => p.ID == id));

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create([FromForm]Product product)
        {
            if (ModelState.IsValid)
            {
                repository.Create(product);
                return RedirectToAction("List");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit([FromRoute]int id) =>
            View(repository.Products.FirstOrDefault(c => c.ID == id));

        [HttpPost]
        public IActionResult Edit([FromRoute]int id, [FromForm]Product product)
        {
            if (!ModelState.IsValid &&
                !repository.Products.Any(p => p.ID == id))
            {
                return View(product);
            }

            product.ID = id;
            repository.Update(product);
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Delete([FromRoute]int id) =>
            View(repository.Products.FirstOrDefault(p => p.ID == id));

        [HttpPost]
        public IActionResult Delete([FromRoute]int id, [FromForm]Product product)
        {
            if (!repository.Products.Any(p => p.ID == id))
            {
                return View();
            }

            product.ID = id;
            repository.Delete(product);

            return RedirectToAction("List");
        }
    }
}