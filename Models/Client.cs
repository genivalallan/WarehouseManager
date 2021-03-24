using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManager.Models
{
    [Table("client")]
    public class Client
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Required]
        [Column("name", TypeName = "varchar(32)")]
        public string Name { get; set; }
        [Column("address", TypeName = "varchar(64)")]
        public string Address { get; set; }
        public virtual ICollection<Stock> Stocks { get; set; }
        public virtual ICollection<Incoming> Incomings { get; set; }
        public virtual ICollection<Shipping> Shippings { get; set; }
    }
}