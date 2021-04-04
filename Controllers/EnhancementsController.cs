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
    public class EnhancementsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public EnhancementsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetEnhancements()
        {
            IEnumerable<Enhancement> enhancements = null;
            PagingInfo pagingInfo = new PagingInfo()
                .Create(repository.Enhancements.Count(), itemsPerPage, HttpContext.Request.Query["page"]);
            
            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1)
                {
                    return Redirect("/enhancements");
                }
                else if (pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return Redirect($"/enhancements?page={pagingInfo.TotalPages}");
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

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(enhancements),
                PagingInfo = pagingInfo
            });
        }
    }
}