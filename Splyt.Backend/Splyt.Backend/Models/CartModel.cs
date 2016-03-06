using System;
using System.Collections.Generic;
using Backend.DataAccess.Models;

namespace Backend.Models
{
    public class CartModel
    {
        public DateTime Date { get; set; }
        public int Id { get; set; }
        public IEnumerable<CartItemModel> Items { get; set; }
        public int MerchantId { get; set; }
        public string MerchantName { get; set; }
        public float PaidPrice { get; set; }
        public PaymentStatus Status { get; set; }
        public float TotalPrice { get; set; }
    }
}