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
    public class IncomingsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public IncomingsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetIncomings()
        {
            IEnumerable<Incoming> incomings = null;
            PagingInfo pagingInfo = new PagingInfo()
                .Create(repository.Incomings.Count(), itemsPerPage, HttpContext.Request.Query["page"]);

            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1)
                {
                    return Redirect("/incomings");
                }
                else if (pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return Redirect($"/incomings?page={pagingInfo.TotalPages}");
                }

                incomings = repository.Incomings
                    .OrderBy(i => i.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(i => i.Client)
                    .Include(i => i.Stock.Product)
                    .Include(i => i.Driver)
                    .Include(i => i.Vehicle)
                    .AsNoTracking();
            }

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(incomings),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetIncoming(int id) =>
            View(repository.Incomings
                .Include(i => i.Client)
                .Include(i => i.Stock.Product)
                .Include(i => i.Driver)
                .Include(i => i.Vehicle)
                .AsNoTracking()
                .FirstOrDefault(i => i.ID == id));
    }
}