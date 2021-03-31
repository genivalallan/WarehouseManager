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
        [Display(Name = "ID do Recibo do Processamento")]
        public int ID { get; set; }

        // [Required]
        // public User User { get; set; }

        [Required(ErrorMessage = "Selecione o estoque do produto base")]
        [Column("base_stock_id")]
        [ForeignKey("BaseStock")]
        [Display(Name = "Produto Processado")]
        public int BaseStockID { get; set; }

        [Required(ErrorMessage = "Selecione o estoque do produto final")]
        [Column("final_stock_id")]
        [ForeignKey("FinalStock")]
        [Display(Name = "Produto Final")]
        public int FinalStockID { get; set; }

        [Required(ErrorMessage = "Selecione o veículo que realizou o transporte")]
        [Column("vehicle_id")]
        [ForeignKey("Vehicle")]
        [Display(Name = "Veículo")]
        public int VehicleID { get; set; }

        [Required(ErrorMessage = "Selecione o motorista que realizou o transporte")]
        [Column("driver_id")]
        [ForeignKey("Driver")]
        [Display(Name = "Motorista")]
        public int DriverID { get; set; }

        [Required(ErrorMessage = "Insira o valor do peso bruto")]
        [Column("gross_wheight")]
        [Display(Name = "Peso Bruto")]
        public int GrossWeight { get; set; }

        [Required]
        [BindNever]
        [Column("net_weight")]
        [Display(Name = "Peso Líquido")]
        public int NetWeight { get; set; }

        [Column("comment")]
        [Display(Name = "Observação")]
        public string Comment { get; set; }

        [Required]
        [BindNever]
        [Column("created_at")]
        [Display(Name = "Data e Hora do Processamento")]
        public DateTime CreatedAt { get; set; }

        public virtual Stock BaseStock { get; set; }
        public virtual Stock FinalStock { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual Driver Driver { get; set; }
    }
}