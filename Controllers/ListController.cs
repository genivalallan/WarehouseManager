using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WarehouseManager.Models;
using WarehouseManager.Models.ViewModels;

namespace WarehouseManager.Controllers
{
    public class ListController : Controller
    {
        private readonly IWMRepository repository;
        private int pageSize = 5;

        public ListController(IWMRepository repo) => repository = repo;

        [HttpGet]
        public IActionResult Clients(int page = 1)
        {
            var pagingInfo = new PagingInfo
            {
                ItemsPerPage = pageSize,
                TotalItems = repository.Clients.Count()
            };

            if (page < 1)
            {
                return Redirect("/List/Clients?page=1");
            }
            else if (page > pagingInfo.TotalPages)
            {
                return Redirect($"/List/Clients?page={pagingInfo.TotalPages}");
            }
            else
            {
                pagingInfo.Page = page;
            }

            return View(new ListViewModel<Client>
            {
                Items = repository.Clients
                    .OrderBy(c => c.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking(),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet]
        public IActionResult Products(int page = 1)
        {
            var pagingInfo = new PagingInfo
            {
                ItemsPerPage = pageSize,
                TotalItems = repository.Products.Count()
            };

            if (page < 1)
            {
                return Redirect("/List/Products?page=1");
            }
            else if (page > pagingInfo.TotalPages)
            {
                return Redirect($"/List/Products?page={pagingInfo.TotalPages}");
            }
            else
            {
                pagingInfo.Page = page;
            }

            return View(new ListViewModel<Product>
            {
                Items = repository.Products
                    .OrderBy(p => p.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking(),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet]
        public IActionResult Stocks(int page = 1)
        {
            var pagingInfo = new PagingInfo
            {
                ItemsPerPage = pageSize,
                TotalItems = repository.Stocks.Count()
            };

            if (page < 1)
            {
                return Redirect("/List/Stocks?page=1");
            }
            else if (page > pagingInfo.TotalPages)
            {
                return Redirect($"/List/Stocks?page={pagingInfo.TotalPages}");
            }
            else
            {
                pagingInfo.Page = page;
            }

            return View(new ListViewModel<Stock>
            {
                Items = repository.Stocks
                    .OrderBy(s => s.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(p => p.Product)
                    .Include(c => c.Owner)
                    .AsNoTracking(),
                PagingInfo = pagingInfo
            });
        }

        [HttpGet]
        public IActionResult Drivers(int page = 1)
        {
            var pagingInfo = new PagingInfo
            {
                ItemsPerPage = pageSize,
                TotalItems = repository.Drivers.Count()
            };

            if (page < 1)
            {
                return Redirect("/List/Drivers?page=1");
            }
            else if (page > pagingInfo.TotalPages)
            {
                return Redirect($"/List/Drivers?page={pagingInfo.TotalPages}");
            }
            else
            {
                pagingInfo.Page = page;
            }

            return View(new ListViewModel<Driver>
            {
                Items = repository.Drivers
                    .OrderBy(d => d.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking(),
                PagingInfo = pagingInfo
            });
        }

        public IActionResult Vehicles(int page = 1)
        {
            var pagingInfo = new PagingInfo
            {
                ItemsPerPage = pageSize,
                TotalItems = repository.Vehicles.Count()
            };

            if (page < 1)
            {
                return Redirect("/List/Vehicles?page=1");
            }
            else if (page > pagingInfo.TotalPages)
            {
                return Redirect($"/List/Vehicles?page={pagingInfo.TotalPages}");
            }
            else
            {
                pagingInfo.Page = page;
            }

            return View(new ListViewModel<Vehicle>
            {
                Items = repository.Vehicles
                    .OrderBy(v => v.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .AsNoTracking(),
                PagingInfo = pagingInfo
            });
        }

        public IActionResult Incomings(int page = 1)
        {
            var pagingInfo = new PagingInfo
            {
                ItemsPerPage = pageSize,
                TotalItems = repository.Incomings.Count()
            };

            if (page < 1)
            {
                return Redirect("/List/Incomings?page=1");
            }
            else if (page > pagingInfo.TotalPages)
            {
                return Redirect($"/List/Incomings?page={pagingInfo.TotalPages}");
            }
            else
            {
                pagingInfo.Page = page;
            }

            return View(new ListViewModel<Incoming>
            {
                Items = repository.Incomings
                    .OrderBy(i => i.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(c => c.Client)
                    .Include(p => p.Stock.Product)
                    .Include(v => v.Vehicle)
                    .Include(d => d.Driver)
                    .AsNoTracking(),
                PagingInfo = pagingInfo
            });
        }

        public IActionResult Shippings(int page = 1)
        {
            var pagingInfo = new PagingInfo
            {
                ItemsPerPage = pageSize,
                TotalItems = repository.Shippings.Count()
            };

            if (page < 1)
            {
                return Redirect("/List/Shippings?page=1");
            }
            else if (page > pagingInfo.TotalPages)
            {
                return Redirect($"/List/Shippings?page={pagingInfo.TotalPages}");
            }
            else
            {
                pagingInfo.Page = page;
            }

            return View(new ListViewModel<Shipping>
            {
                Items = repository.Shippings
                    .OrderBy(s => s.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(c => c.Client)
                    .Include(p => p.Stock.Product)
                    .Include(v => v.Vehicle)
                    .Include(d => d.Driver)
                    .AsNoTracking(),
                PagingInfo = pagingInfo
            });
        }

        public IActionResult Enhancements(int page = 1)
        {
            var pagingInfo = new PagingInfo
            {
                ItemsPerPage = pageSize,
                TotalItems = repository.Enhancements.Count()
            };

            if (page < 1)
            {
                return Redirect("/List/Enhancements?page=1");
            }
            else if (page > pagingInfo.TotalPages)
            {
                return Redirect($"/List/Enhancements?page={pagingInfo.TotalPages}");
            }
            else
            {
                pagingInfo.Page = page;
            }

            return View(new ListViewModel<Enhancement>
            {
                Items = repository.Enhancements
                    .OrderBy(e => e.ID)
                    .Skip((pagingInfo.Page - 1) * pagingInfo.ItemsPerPage)
                    .Take(pagingInfo.ItemsPerPage)
                    .Include(p => p.BaseStock.Product)
                    .Include(p => p.FinalStock.Product)
                    .Include(v => v.Vehicle)
                    .Include(d => d.Driver)
                    .AsNoTracking(),
                PagingInfo = pagingInfo
            });
        }
    }
}