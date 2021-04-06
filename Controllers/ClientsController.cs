using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WarehouseManager.Models;
using WarehouseManager.Models.ViewModels;
using WarehouseManager.Infrastructure;

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
            IEnumerable<Client> clients = null;
            PagingInfo pagingInfo = new PagingInfo()
                .Create(repository.Clients.Count(), itemsPerPage, HttpContext.Request.Query["page"]);

            if (pagingInfo.TotalItems != 0)
            {
                if (pagingInfo.Page < 1)
                {
                    return Redirect("/clients");
                }
                else if (pagingInfo.Page > pagingInfo.TotalPages)
                {
                    return Redirect($"/clients?page={pagingInfo.TotalPages}");
                }

                clients = repository.Clients
                    .OrderBy(c => c.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking();
            }

            return View(new ListViewModel
            {
                JsonItems = JsonSerializer.Serialize(clients),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetClient(int id) =>
            View(repository.Clients.FirstOrDefault(c => c.ID == id));
    }
}