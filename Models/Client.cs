using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WarehouseManager.Models
{
    [Table("client")]
    public class Client
    {
        [Key]
        [BindNever]
        [Column("id")]
        [Display(Name = "ID do Cliente")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Insira o nome do cliente")]
        [Column("name")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Column("address")]
        [Display(Name = "Endereço")]
        public string Address { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<Stock> Stocks { get; set; }
        [JsonIgnore]
        public virtual ICollection<Incoming> Incomings { get; set; }
        [JsonIgnore]
        public virtual ICollection<Shipping> Shippings { get; set; }
    }
}