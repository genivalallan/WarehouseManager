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
    }

    public static class PagingInfoExtension
    {
        public static PagingInfo Create(this PagingInfo p, int totalItems, int itemsPerPage, string query)
        {
            if (query == "" ||
                !Int32.TryParse(query, out int page))
            {
                page = 1;
            }

            p.TotalItems = totalItems;
            p.ItemsPerPage = itemsPerPage;
            p.Page = page;

            return p;
        }
    }
}