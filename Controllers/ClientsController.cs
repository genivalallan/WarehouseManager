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
    public class ClientsController : Controller
    {
        private readonly IWMRepository repository;
        private int itemsPerPage = 5;

        public ClientsController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult GetClients()
        {
            int page;
            if (HttpContext.Request.Query["page"] == "" ||
                !Int32.TryParse(HttpContext.Request.Query["page"], out page))
            {
                page = 1;
            }

            IEnumerable<Client> clients = null;
            PagingInfo pagingInfo = null;
            if (repository.Clients.Count() != 0)
            {
                pagingInfo = new PagingInfo
                {
                    ItemsPerPage = itemsPerPage,
                    TotalItems = repository.Clients.Count()
                };

                if (page < 1)
                {
                    return Redirect("/clients");
                }
                else if (page > pagingInfo.TotalPages)
                {
                    return Redirect($"/clients?page={pagingInfo.TotalPages}");
                }
                else
                {
                    pagingInfo.Page = page;
                }

                clients = repository.Clients
                    .OrderBy(c => c.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking();
            }

            return View(new ListViewModel<Client>
            {
                Items = clients,
                PagingInfo = pagingInfo
            });
        }
    }
}