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
    public class StocksController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public StocksController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetStocks()
        {
            int page;
            if (HttpContext.Request.Query["page"] == "" ||
                !Int32.TryParse(HttpContext.Request.Query["page"], out page))
            {
                page = 1;
            }

            IEnumerable<Stock> stocks = null;
            PagingInfo pagingInfo = null;
            if (repository.Stocks.Count() != 0)
            {
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = itemsPerPage,
                    TotalItems = repository.Stocks.Count()
                };

                if (page < 1)
                {
                    return Redirect("/stocks");
                }
                else if (page > pagingInfo.TotalPages)
                {
                    return Redirect($"/stocks?page={pagingInfo.TotalPages}");
                }
                else
                {
                    pagingInfo.Page = page;
                }

                stocks = repository.Stocks
                    .OrderBy(s => s.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(p => p.Product)
                    .Include(c => c.Owner)
                    .AsNoTracking();
            }

            return View(new ListViewModel<Stock>
            {
                Items = stocks,
                PagingInfo = pagingInfo
            });
        }
    }
}