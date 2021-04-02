using System.Collections.Generic;

using WarehouseManager.Models;

namespace WarehouseManager.Models.ViewModels
{
    public class PagingListViewModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}