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
                    .Include(e => e.BaseStock.Product)
                    .Include(e => e.FinalStock.Product)
                    .Include(e => e.Driver)
                    .Include(e => e.Vehicle)
                    .AsNoTracking();
            }

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(enhancements),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetEnhancement(int id) =>
            View(repository.Enhancements
                .Include(e => e.BaseStock.Product)
                .Include(e => e.FinalStock.Product)
                .Include(e => e.Driver)
                .Include(e => e.Vehicle)
                .AsNoTracking()
                .FirstOrDefault(e => e.ID == id));
    }
}