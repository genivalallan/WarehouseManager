using System;

namespace WarehouseManager.Infrastructure
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int Page { get; set; }
        public int TotalPages =>
            (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

        public PagingInfo() {}

        public PagingInfo(int totalItems, int itemsPerPage, int page)
        {
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;
            Page = page;
        }
    }
}