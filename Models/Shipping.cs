using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WarehouseManager.Models
{
    [Table("shipping")]
    public class Shipping
    {
        [Key]
        [BindNever]
        [Column("id")]
        [Display(Name = "ID do Recibo de Saída")]
        public int ID { get; set; }

        // [Required]
        // public User User { get; set; }

        [Required(ErrorMessage = "Selecione o estoque de saída")]
        [Column("stock_id")]
        [ForeignKey("Stock")]
        [Display(Name = "Origem")]
        public int StockID { get; set; }

        [Required(ErrorMessage = "Selecione o cliente de destino")]
        [Column("client_id")]
        [ForeignKey("Client")]
        [Display(Name = "Destino")]
        public int ClientID { get; set; }

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
        [Display(Name = "Data e Hora da Saída")]
        public DateTime CreatedAt { get; set; }

        public virtual Stock Stock { get; set; }
        public virtual Client Client { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual Driver Driver { get; set; }
    }
}