using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WarehouseManager.Models
{
    [Table("product")]
    public class Product
    {
        [Key]
        [BindNever]
        [Column("id")]
        public int ID { get; set; }
        [Required]
        [Column("name", TypeName = "varchar(32)")]
        public string Name { get; set; }
        [Column("description", TypeName = "varchar(64)")]
        public string Description { get; set; }
        [BindNever]
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}