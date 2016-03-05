using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataAccess.Models
{
    public class Product : Model
    {
        public string Description { get; set; }

        public virtual Merchant Merchant { get; set; }

        [ForeignKey("Merchant")]
        public int MerchantId { get; set; }

        public float Price { get; set; }
    }
}