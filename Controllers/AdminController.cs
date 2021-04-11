using Microsoft.AspNetCore.Mvc;

using WarehouseManager.Models;

namespace WarehouseManager.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWMRepository repository;

        public AdminController(IWMRepository repo) => repository = repo;

        public IActionResult DbInit() => View(repository.DbInit(HttpContext.Request.Query["sample"] == "true"));
    }
}