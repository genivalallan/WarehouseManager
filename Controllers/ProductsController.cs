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
    public class ProductsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public ProductsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetProducts()
        {
            int page;
            if (HttpContext.Request.Query["page"] == "" ||
                !Int32.TryParse(HttpContext.Request.Query["page"], out page))
            {
                page = 1;
            }

            IEnumerable<Product> products = null;
            PagingInfo pagingInfo = null;
            if (repository.Products.Count() != 0)
            {
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = itemsPerPage,
                    TotalItems = repository.Products.Count()
                };

                if (page < 1)
                {
                    return Redirect("/products");
                }
                else if (page > pagingInfo.TotalPages)
                {
                    return Redirect($"/products?page={pagingInfo.TotalPages}");
                }
                else
                {
                    pagingInfo.Page = page;
                }

                products = repository.Products
                    .OrderBy(p => p.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking();
            }

            return View(new ListViewModel<Product>
            {
                Items = products,
                PagingInfo = pagingInfo
            });
        }
    }
}