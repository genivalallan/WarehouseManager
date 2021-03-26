using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WarehouseManager.Models
{
    [Table("driver")]
    public class Driver
    {
        [Key]
        [BindNever]
        [Column("id")]
        [Display(Name = "ID do Motorista")]
        public int ID { get; set; }

        [Required]
        [Column("name", TypeName = "varchar(32)")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Column("cnh", TypeName = "varchar(11)")]
        [Display(Name = "CNH")]
        public string CNH { get; set; }
        
        [BindNever]
        public virtual ICollection<Incoming> Incomings { get; set; }
        [BindNever]
        public virtual ICollection<Shipping> Shippings { get; set; }
        [BindNever]
        public virtual ICollection<Enhancement> Enhancements { get; set; }
    }
}