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
    public class StocksController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public StocksController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetStocks()
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
                    .Include(p => p.Product)
                    .Include(c => c.Owner)
                    .AsNoTracking();
            }

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(stocks),
                PagingInfo = pagingInfo
            });
        }
    }
}