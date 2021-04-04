using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WarehouseManager.Models
{
    [Table("stock")]
    public class Stock
    {
        [Key]
        [BindNever]
        [Column("id")]
        [Display(Name = "ID do Estoque")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Selecione um produto para criar o estoque")]
        [Column("product_id")]
        [ForeignKey("Product")]
        [Display(Name = "Produto")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Selecione o proprietário do estoque")]
        [Column("client_id")]
        [ForeignKey("Owner")]
        [Display(Name = "Proprietário")]
        public int ClientID { get; set; }

        [BindNever]
        [Column("balance")]
        [Display(Name = "Saldo")]
        public int Balance { get; set; }

        public virtual Product Product { get; set; }
        public virtual Client Owner { get; set; }

        [JsonIgnore]
        public virtual ICollection<Incoming> Incomings { get; set; }
        [JsonIgnore]
        public virtual ICollection<Shipping> Shippings { get; set; }
        [JsonIgnore]
        public virtual ICollection<Enhancement> EnhancementBaseStocks { get; set; }
        [JsonIgnore]
        public virtual ICollection<Enhancement> EnhancementFinalStocks { get; set; }
    }
}