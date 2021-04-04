using System.Text.Json.Serialization;
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

        [Required(ErrorMessage = "Insira o nome do motorista")]
        [Column("name")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Insira o número da CNH do motorista")]
        [Column("cnh")]
        [Display(Name = "CNH")]
        public string CNH { get; set; }

        [JsonIgnore]
        public virtual ICollection<Incoming> Incomings { get; set; }
        [JsonIgnore]
        public virtual ICollection<Shipping> Shippings { get; set; }
        [JsonIgnore]
        public virtual ICollection<Enhancement> Enhancements { get; set; }
    }
}