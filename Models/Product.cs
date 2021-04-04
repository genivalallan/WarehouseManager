using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WarehouseManager.Models
{
    [Table("product")]
    public class Product
    {
        [Key]
        [BindNever]
        [Column("id")]
        [Display(Name = "ID do Produto")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Insira o nome do produto")]
        [Column("name")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Column("description")]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}