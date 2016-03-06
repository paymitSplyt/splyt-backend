using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataAccess.Models
{
    public class CartItem_User : Model
    {
        public int Amount { get; set; }

        public virtual CartItem CartItem { get; set; }

        [ForeignKey("CartItem")]
        public int CartItemId { get; set; }

        public PaymentStatus Status { get; set; }

        public virtual User User { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
    }
}