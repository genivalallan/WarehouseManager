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
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public ProductsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetProducts()
        {
            IEnumerable<Product> products = null;
            PagingInfo pagingInfo = new PagingInfo()
                .Create(repository.Products.Count(), itemsPerPage, HttpContext.Request.Query["page"]);
            
            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1)
                {
                    return Redirect("/products");
                }
                else if (pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return Redirect($"/products?page={pagingInfo.TotalPages}");
                }

                products = repository.Products
                    .OrderBy(p => p.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking();
            }

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(products),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id) =>
            View(repository.Products.FirstOrDefault(p => p.ID == id));
    }
}