using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WarehouseManager.Models
{
    [Table("vehicle")]
    public class Vehicle
    {
        [Key]
        [BindNever]
        [Column("id")]
        [Display(Name = "ID do Veículo")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Insira a placa do veículo")]
        [Column("plate1")]
        [Display(Name = "Placa do Cavalo")]
        public string Plate1 { get; set; }

        [Column("plate2")]
        [Display(Name = "Placa da Carreta")]
        public string Plate2 { get; set; }

        [Column("plate3")]
        [Display(Name = "Placa da Carreta")]
        public string Plate3 { get; set; }

        [Column("rntrc")]
        [Display(Name = "RNTRC")]
        public string RNTRC { get; set; }

        [Required(ErrorMessage = "Insira o valor da tara do veículo")]
        [Column("tare")]
        [Display(Name = "Tara")]
        public int Tare { get; set; }

        [JsonIgnore]
        public virtual ICollection<Incoming> Incomings { get; set; }
        [JsonIgnore]
        public virtual ICollection<Shipping> Shippings { get; set; }
        [JsonIgnore]
        public virtual ICollection<Enhancement> Enhancements { get; set; }
    }
}