using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WarehouseManager.Models
{
    [Table("client")]
    public class Client
    {
        [Key]
        [BindNever]
        [Column("id")]
        [Display(Name = "ID do Cliente")]
        public int ID { get; set; }

        [Required]
        [Column("name", TypeName = "varchar(32)")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Column("address", TypeName = "varchar(64)")]
        [Display(Name = "Endereço")]
        public string Address { get; set; }
        
        [BindNever]
        public virtual ICollection<Stock> Stocks { get; set; }
        [BindNever]
        public virtual ICollection<Incoming> Incomings { get; set; }
        [BindNever]
        public virtual ICollection<Shipping> Shippings { get; set; }
    }
}