using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataAccess.Models
{
    public class CartItem : Model
    {
        public int Amount { get; set; }

        public virtual Cart Cart { get; set; }

        [ForeignKey("Cart")]
        [Index("IX_Item_Unique", IsUnique = true, Order = 1)]
        public int CartId { get; set; }

        public virtual ICollection<CartItem_User> CartItem_Users { get; set; }

        public Product Product { get; set; }

        [ForeignKey("Product")]
        [Index("IX_Item_Unique", IsUnique = true, Order = 2)]
        public int ProductId { get; set; }

        public PaymentStatus Status { get; set; }
    }
}