using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataAccess.Models
{
    public class Cart : Model
    {
        public virtual ICollection<CartItem> CartItems { get; set; }

        public virtual Merchant Merchant { get; set; }

        [ForeignKey("Merchant")]
        public int MerchantId { get; set; }
    }
}