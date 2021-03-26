using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WarehouseManager.Models
{
    [Table("enhancement")]
    public class Enhancement
    {
        [Key]
        [BindNever]
        [Column("id")]
        [Display(Name = "ID do Processamento")]
        public int ID { get; set; }

        // [Required]
        // public User User { get; set; }

        [Required]
        [ForeignKey("stock_origin_id")]
        [Display(Name = "Origem")]
        public virtual Stock BaseProduct { get; set; }

        [Required]
        [ForeignKey("stock_destination_id")]
        [Display(Name = "Destino")]
        public virtual Stock FinalProduct { get; set; }

        [Required]
        [ForeignKey("vehicle_id")]
        [Display(Name = "Veículo")]
        public virtual Vehicle Vehicle { get; set; }

        [Required]
        [ForeignKey("driver_id")]
        [Display(Name = "Motorista")]
        public virtual Driver Driver { get; set; }

        [Required]
        [Column("gross_wheight")]
        [Display(Name = "Peso Bruto")]
        public int GrossWeight { get; set; }

        [Required]
        [Column("vehicle_tare")]
        [Display(Name = "Tara do Veículo")]
        public int VehicleTare { get; set; }

        [Required]
        [Column("net_weight")]
        [Display(Name = "Peso Líquido")]
        public int NetWeight { get; set; }

        [Column("description", TypeName = "varchar(128)")]
        [Display(Name = "Observação")]
        public string Description { get; set; }

        [Required]
        [Column("created_at")]
        [Display(Name = "Data e Hora do Processamento")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("status")]
        [Display(Name = "Status do Processamento")]
        public OpStatus Status { get; set; }
    }
}