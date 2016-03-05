using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Backend.DataAccess.Models;
using Backend.Models;
using EntityFramework.Extensions;

namespace Backend.Services
{
    public class CartService : Service
    {
        public bool AddUserToItem(int itemId, long phonenumber)
        {
            using (var ts = new TransactionScope())
            {
                var user = GetOrCreateUser(phonenumber);
                var map = DataContext.RequestItemUsers.FirstOrDefault(x => x.CartItemId == itemId && x.UserId == user.Id);

                if (map != null)
                {
                    return true;
                }

                map = new CartItem_User
                {
                    CartItemId = itemId,
                    Status = PaymentStatus.Open,
                    UserId = user.Id
                };
                DataContext.RequestItemUsers.Add(map);
                var count = DataContext.SaveChanges();
                ts.Complete();
                return count > 0;
            }
        }

        public int CreateCart(int merchantId)
        {
            var request = new Cart
            {
                MerchantId = merchantId
            };
            DataContext.Requests.Add(request);
            DataContext.SaveChanges();
            return request.Id;
        }

        public int CreateItem(CartItem item)
        {
            DataContext.RequestItems.Add(item);
            DataContext.SaveChanges();
            return item.Id;
        }

        public bool DeleteItem(int itemId)
        {
            var item = DataContext.RequestItems.Find(itemId);
            if (item != null)
            {
                DataContext.RequestItemUsers.Where(x => x.CartItemId == itemId).Delete();
                DataContext.RequestItems.Remove(item);
                return DataContext.SaveChanges() > 0;
            }
            return false;
        }

        public CartModel GetCart(int id)
        {
            var query = GetCartsQueryable();
            var result = query.FirstOrDefault(request => request.Id == id);
            if (result != null)
            {
                result.Status = result.Items.Any(item => item.Status == PaymentStatus.Open)
                    ? PaymentStatus.Open
                    : result.Items.All(item => item.Status == PaymentStatus.Canceled) ? PaymentStatus.Canceled : PaymentStatus.Done;
            }
            return result;
        }

        public IEnumerable<CartModel> GetCarts(int merchantId)
        {
            var query = GetCartsQueryable();
            return query.Where(cart => cart.MerchantId == merchantId).ToArray();
        }

        public User GetOrCreateUser(long phonenumber)
        {
            var user = DataContext.Users.FirstOrDefault(u => u.Phonenumber == phonenumber);
            if (user == null)
            {
                user = new User { Phonenumber = phonenumber };
                DataContext.Users.Add(user);
                DataContext.SaveChanges();
            }
            return user;
        }

        public object PayCart(int cartId, int userId)
        {
            using (var ts = new TransactionScope())
            {
                var maps = DataContext.RequestItemUsers.Where(map => map.UserId == userId && map.CartItem.CartId == cartId);
                // make payment...
                foreach (var cartItemUser in maps)
                {
                    cartItemUser.Status = PaymentStatus.Done;
                }
            }
            throw new NotImplementedException();
        }

        public bool RemoveUserFromItem(int itemId, long phonenumber)
        {
            using (var ts = new TransactionScope())
            {
                var user = GetOrCreateUser(phonenumber);
                var existing = DataContext.RequestItemUsers.FirstOrDefault(x => x.CartItemId == itemId && x.UserId == user.Id);
                if (existing == null)
                {
                    return true;
                }

                if (existing.Status != PaymentStatus.Open)
                {
                    return false;
                }

                DataContext.RequestItemUsers.Remove(existing);
                var count = DataContext.SaveChanges();
                ts.Complete();
                return count > 0;
            }
        }

        private IQueryable<CartModel> GetCartsQueryable()
        {
            return from request in DataContext.Requests
                   select new CartModel
                   {
                       Id = request.Id,
                       Items = from item in request.CartItems
                               select new CartItemModel
                               {
                                   Amount = item.Amount,
                                   Description = item.Product.Description,
                                   Price = item.Product.Price,
                                   ProductId = item.ProductId,
                                   Users = from ciu in item.CartItem_Users
                                           select new CartItemUserModel
                                           {
                                               Amount = ciu.Amount,
                                               Phonenumber = ciu.User.Phonenumber
                                           },
                                   Status = item.CartItem_Users.Count > 1 ? item.CartItem_Users.Select(riu => riu.Status).Max() : item.Status,
                               },
                       MerchantId = request.MerchantId,
                       MerchantName = request.Merchant.Name,
                       Date = request.Created,
                       TotalPrice = request.CartItems.Sum(x => x.Amount * x.Product.Price)
                   };
        }
    }
}