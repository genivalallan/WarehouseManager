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
    public class ShippingsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public ShippingsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetShippings()
        {
            IEnumerable<Shipping> shippings = null;
            PagingInfo pagingInfo = new PagingInfo()
                .Create(repository.Incomings.Count(), itemsPerPage, HttpContext.Request.Query["page"]);
            
            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1)
                {
                    return Redirect("/shippings");
                }
                else if (pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return Redirect($"/shippings?page={pagingInfo.TotalPages}");
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

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(shippings),
                PagingInfo = pagingInfo
            });
        }
    }
}