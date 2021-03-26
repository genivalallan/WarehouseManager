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
        [Display(Name = "ID do Produto")]
        public int ID { get; set; }

        [Required]
        [Column("name", TypeName = "varchar(32)")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Column("description", TypeName = "varchar(64)")]
        [Display(Name = "Descrição")]
        public string Description { get; set; }
        
        [BindNever]
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}