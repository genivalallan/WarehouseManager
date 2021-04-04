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
    public class EnhancementsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public EnhancementsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetEnhancements()
        {
            int page;
            if (HttpContext.Request.Query["page"] == "" ||
                !Int32.TryParse(HttpContext.Request.Query["page"], out page))
            {
                page = 1;
            }

            IEnumerable<Enhancement> enhancements = null;
            PagingInfo pagingInfo = null;
            if (repository.Enhancements.Count() != 0)
            {
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = itemsPerPage,
                    TotalItems = repository.Enhancements.Count()
                };

                if (page < 1)
                {
                    return Redirect("/enhancements");
                }
                else if (page > pagingInfo.TotalPages)
                {
                    return Redirect($"/enhancements?page={pagingInfo.TotalPages}");
                }
                else
                {
                    pagingInfo.Page = page;
                }

                enhancements = repository.Enhancements
                    .OrderBy(e => e.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(s => s.BaseStock.Product)
                    .Include(s => s.FinalStock.Product)
                    .Include(d => d.Driver)
                    .Include(v => v.Vehicle)
                    .AsNoTracking();
            }

            return View(new ListViewModel<Enhancement>
            {
                Items = enhancements,
                PagingInfo = pagingInfo
            });
        }
    }
}