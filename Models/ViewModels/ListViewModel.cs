using System.Collections.Generic;

using WarehouseManager.Models;
using WarehouseManager.Infrastructure;

namespace WarehouseManager.Models.ViewModels
{
    public class ListViewModel
    {
        public string JsonItems { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}