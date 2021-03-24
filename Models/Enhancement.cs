using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManager.Models
{
    [Table("enhancement")]
    public class Enhancement
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        // [Required]
        // public User User { get; set; }
        [Required]
        [ForeignKey("stock_origin_id")]
        public virtual Stock BaseProduct { get; set; }
        [Required]
        [ForeignKey("stock_destination_id")]
        public virtual Stock FinalProduct { get; set; }
        [Required]
        [ForeignKey("vehicle_id")]
        public virtual Vehicle Vehicle { get; set; }
        [Required]
        [ForeignKey("driver_id")]
        public virtual Driver Driver { get; set; }
        [Required]
        [Column("gross_wheight")]
        public int GrossWeight { get; set; }
        [Required]
        [Column("vehicle_tare")]
        public int VehicleTare { get; set; }
        [Required]
        [Column("net_weight")]
        public int NetWeight { get; set; }
        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Required]
        [Column("status")]
        public OpStatus Status { get; set; }
    }
}