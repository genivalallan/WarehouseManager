using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManager.Models
{
    [Table("stock")]
    public class Stock
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Required]
        [ForeignKey("product_id")]
        public virtual Product Product { get; set; }
        [Required]
        [ForeignKey("client_id")]
        public virtual Client Owner { get; set; }
        [Required]
        [Column("balance")]
        public int Balance { get; set; }
        public virtual ICollection<Incoming> Incomings { get; set; }
        public virtual ICollection<Shipping> Shippings { get; set; }
        public virtual ICollection<Enhancement> EnhancementBaseProducts { get; set; }
        public virtual ICollection<Enhancement> EnhancementFinalProducts { get; set; }
    }
}