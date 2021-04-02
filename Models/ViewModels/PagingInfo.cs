using System;

namespace WarehouseManager.Models.ViewModels
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int Page { get; set; }
        public int TotalPages =>
            (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
    }
}