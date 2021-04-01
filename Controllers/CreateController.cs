using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using WarehouseManager.Models;

namespace WarehouseManager.Controllers
{
    public class CreateController : Controller
    {
        private readonly IWMRepository repository;

        public CreateController(IWMRepository repo) => repository = repo;

        public IActionResult Success() => View();

        [HttpGet]
        public IActionResult Client() => View();

        [HttpPost]
        public IActionResult Client(Client client)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            repository.Add(client);
            return RedirectToAction("Success");
        }

        [HttpGet]
        public IActionResult Driver() => View();

        [HttpPost]
        public IActionResult Driver(Driver driver)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            repository.Add(driver);
            return RedirectToAction("Success");
        }

        [HttpGet]
        public IActionResult Enhancement() => View();

        [HttpPost]
        public IActionResult Enhancement(Enhancement enhancement)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            repository.Add(enhancement);
            return RedirectToAction("Success");
        }

        [HttpGet]
        public IActionResult Incoming() => View();

        [HttpPost]
        public IActionResult Incoming(Incoming newIncoming)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            repository.Add(newIncoming);
            return RedirectToAction("Success");
        }

        [HttpGet]
        public IActionResult Product() => View();

        [HttpPost]
        public IActionResult Product(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            repository.Add(product);
            return RedirectToAction("Success");
        }

        [HttpGet]
        public IActionResult Shipping() => View();

        [HttpPost]
        public IActionResult Shipping(Shipping shipping)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            repository.Add(shipping);
            return RedirectToAction("Success");
        }

        [HttpGet]
        public IActionResult Stock() => View();

        [HttpPost]
        public IActionResult Stock(Stock stock)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            repository.Add(stock);
            return RedirectToAction("Success");
        }

        [HttpGet]
        public IActionResult Vehicle() => View();

        [HttpPost]
        public IActionResult Vehicle(Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            repository.Add(vehicle);
            return RedirectToAction("Success");
        }
    }
}