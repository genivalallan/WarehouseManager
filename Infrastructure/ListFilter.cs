using System;

namespace WarehouseManager.Infrastructure
{
    public class ListFilter
    {
        public string Order { get; set; }
        public string OrderBy { get; set; }
        public string SearchBy { get; set; }
        public string Search { get; set; }
        public string InitDate { get; set; }
        public string EndDate { get; set; }
    }
}