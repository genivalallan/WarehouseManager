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
    public class IncomingsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public IncomingsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetIncomings()
        {
            int page;
            if (HttpContext.Request.Query["page"] == "" ||
                !Int32.TryParse(HttpContext.Request.Query["page"], out page))
            {
                page = 1;
            }

            IEnumerable<Incoming> incomings = null;
            PagingInfo pagingInfo = null;
            if (repository.Incomings.Count() != 0)
            {
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = itemsPerPage,
                    TotalItems = repository.Incomings.Count()
                };

                if (page < 1)
                {
                    return Redirect("/incomings");
                }
                else if (page > pagingInfo.TotalPages)
                {
                    return Redirect($"/incomings?page={pagingInfo.TotalPages}");
                }
                else
                {
                    pagingInfo.Page = page;
                }

                incomings = repository.Incomings
                    .OrderBy(i => i.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(c => c.Client)
                    .Include(s => s.Stock.Product)
                    .Include(d => d.Driver)
                    .Include(v => v.Vehicle)
                    .AsNoTracking();
            }

            return View(new ListViewModel<Incoming>
            {
                Items = incomings,
                PagingInfo = pagingInfo
            });
        }
    }
}