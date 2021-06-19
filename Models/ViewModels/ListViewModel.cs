using WarehouseManager.Infrastructure;

namespace WarehouseManager.Models.ViewModels
{
    public class ListViewModel
    {
        public string JsonItems { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public ListFilter ListFilter { get; set; }
    }
}