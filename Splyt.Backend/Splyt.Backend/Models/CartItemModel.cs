using System.Collections.Generic;
using Backend.DataAccess.Models;

namespace Backend.Models
{
    public class CartItemModel
    {
        public int Amount { get; set; }
        public int CartId { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public float Price { get; set; }
        public int ProductId { get; set; }
        public int SelectedAmount { get; set; }
        public PaymentStatus Status { get; set; }
        public int UserAmount { get; set; }
        public IEnumerable<CartItemUserModel> Users { get; set; }
    }
}