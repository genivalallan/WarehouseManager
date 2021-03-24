using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManager.Models
{
    [Table("driver")]
    public class Driver
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Required]
        [Column("name", TypeName = "varchar(32)")]
        public string Name { get; set; }
        [Column("cnh", TypeName = "varchar(11)")]
        public string CNH { get; set; }
        public virtual ICollection<Incoming> Incomings { get; set; }
        public virtual ICollection<Shipping> Shippings { get; set; }
        public virtual ICollection<Enhancement> Enhancements { get; set; }
    }
}