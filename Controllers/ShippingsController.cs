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
    public class ShippingsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public ShippingsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetShippings()
        {
            int page;
            if (HttpContext.Request.Query["page"] == "" ||
                !Int32.TryParse(HttpContext.Request.Query["page"], out page))
            {
                page = 1;
            }

            IEnumerable<Shipping> shippings = null;
            PagingInfo pagingInfo = null;
            if (repository.Shippings.Count() != 0)
            {
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = itemsPerPage,
                    TotalItems = repository.Shippings.Count()
                };

                if (page < 1)
                {
                    return Redirect("/shippings");
                }
                else if (page > pagingInfo.TotalPages)
                {
                    return Redirect($"/shippings?page={pagingInfo.TotalPages}");
                }
                else
                {
                    pagingInfo.Page = page;
                }

                shippings = repository.Shippings
                    .OrderBy(s => s.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(s => s.Stock.Product)
                    .Include(c => c.Client)
                    .Include(d => d.Driver)
                    .Include(v => v.Vehicle)
                    .AsNoTracking();
            }

            return View(new ListViewModel<Shipping>
            {
                Items = shippings,
                PagingInfo = pagingInfo
            });
        }
    }
}