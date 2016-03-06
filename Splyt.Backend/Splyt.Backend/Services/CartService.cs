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
        public void AddUserToItem(int itemId, long phonenumber)
        {
            using (var ts = new TransactionScope())
            {
                var maps = DataContext.CartItemUsers.Where(x => x.CartItemId == itemId).Sum(cui => (int?)cui.Amount);
                var item = DataContext.CartItems.Find(itemId);
                if (item.Amount <= maps)
                {
                    return;
                }

                var user = GetOrCreateUser(phonenumber);
                var map = DataContext.CartItemUsers.FirstOrDefault(x => x.CartItemId == itemId && x.UserId == user.Id);

                if (map != null)
                {
                    return;
                }

                map = new CartItem_User
                {
                    Amount = 1,
                    CartItemId = itemId,
                    Status = PaymentStatus.Open,
                    UserId = user.Id
                };
                DataContext.CartItemUsers.Add(map);
                DataContext.SaveChanges();
                ts.Complete();
            }
        }

        public int CreateCart(int merchantId)
        {
            var request = new Cart
            {
                MerchantId = merchantId
            };
            DataContext.Carts.Add(request);
            DataContext.SaveChanges();
            return request.Id;
        }

        public int CreateItem(int cartId, int productId, int amount)
        {
            var item = DataContext.CartItems.FirstOrDefault(i => i.CartId == cartId && i.ProductId == productId);

            if (item != null)
            {
                item.Amount += amount;
            }
            else
            {
                item = new CartItem
                {
                    Amount = amount,
                    CartId = cartId,
                    ProductId = productId,
                    Status = PaymentStatus.Open
                };
                DataContext.CartItems.Add(item);
            }

            DataContext.SaveChanges();
            return item.Id;
        }

        public int CreateItem(int cartId, string description, float price, int amount)
        {
            using (var ts = new TransactionScope())
            {
                var cart = DataContext.Carts.Find(cartId);
                var product =
                    DataContext.Products.FirstOrDefault(p => p.MerchantId == cart.MerchantId && p.Description == description && p.Price == price);
                if (product == null)
                {
                    product = new Product
                    {
                        Description = description,
                        MerchantId = cart.MerchantId,
                        Price = price
                    };
                    DataContext.Products.Add(product);
                    DataContext.SaveChanges();
                }
                var id = CreateItem(cartId, product.Id, amount);
                ts.Complete();
                return id;
            }
        }

        public void DeleteItem(int itemId)
        {
            var item = DataContext.CartItems.Find(itemId);
            if (item != null)
            {
                DataContext.CartItemUsers.Where(x => x.CartItemId == itemId).Delete();
                DataContext.CartItems.Remove(item);
                DataContext.SaveChanges();
            }
        }

        public float GetBalance(int cartId, long phonenumber)
        {
            var user = GetOrCreateUser(phonenumber);
            var query = from item in DataContext.CartItems
                        where item.CartId == cartId
                        where item.CartItem_Users.Any(ciu => ciu.UserId == user.Id)
                        select new
                        {
                            Price = item.Product.Price * item.CartItem_Users.Where(ciu => ciu.UserId == user.Id).Sum(ciu => ciu.Amount)
                        };
            return query.Sum(x => (float?)x.Price) ?? 0;
        }

        public CartModel GetCart(int id, long? phonenumber = null)
        {
            var query = GetCartsQueryable(phonenumber);
            var result = query.FirstOrDefault(cart => cart.Id == id);
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

        public IEnumerable<long> GetUsers(int cartId)
        {
            var query = from ciu in DataContext.CartItemUsers
                        where ciu.CartItem.CartId == cartId
                        select ciu.User.Phonenumber;
            return query.Distinct().ToArray();
        }

        public object PayCart(int cartId, int userId)
        {
            using (var ts = new TransactionScope())
            {
                var maps = DataContext.CartItemUsers.Where(map => map.UserId == userId && map.CartItem.CartId == cartId);
                // make payment...
                foreach (var cartItemUser in maps)
                {
                    cartItemUser.Status = PaymentStatus.Done;
                }
            }
            throw new NotImplementedException();
        }

        public void RemoveUserFromItem(int itemId, long phonenumber)
        {
            using (var ts = new TransactionScope())
            {
                var user = GetOrCreateUser(phonenumber);
                var existing = DataContext.CartItemUsers.FirstOrDefault(x => x.CartItemId == itemId && x.UserId == user.Id);

                if (existing?.Status != PaymentStatus.Open)
                {
                    return;
                }

                DataContext.CartItemUsers.Remove(existing);
                DataContext.SaveChanges();
                ts.Complete();
            }
        }

        public void SetItemAmount(int itemId, long phonenumber, int amount)
        {
            using (var ts = new TransactionScope())
            {
                var user = GetOrCreateUser(phonenumber);

                var map = DataContext.CartItemUsers.FirstOrDefault(x => x.CartItemId == itemId && x.UserId == user.Id);
                if (map == null)
                {
                    return;
                }

                var otherMapsAmount = DataContext.CartItemUsers.Where(x => x.CartItemId == itemId && x.UserId != user.Id).Sum(cui => (int?)cui.Amount);
                var item = DataContext.CartItems.Find(itemId);
                if (amount < 0 || otherMapsAmount + amount > item.Amount)
                {
                    return;
                }

                if (amount == 0)
                {
                    DataContext.CartItemUsers.Remove(map);
                }
                else if (map.CartItem.Amount >= amount)
                {
                    map.Amount = amount;
                }

                DataContext.SaveChanges();
                ts.Complete();
            }
        }

        private IQueryable<CartModel> GetCartsQueryable(long? phonenumber = null)
        {
            int? userId = null;
            if (phonenumber.HasValue)
            {
                var user = GetOrCreateUser(phonenumber.Value);
                userId = user.Id;
            }
            Console.WriteLine("userid " + userId);
            return from cart in DataContext.Carts
                   select new CartModel
                   {
                       Id = cart.Id,
                       Items = from item in cart.CartItems
                               where userId == null ||
                                     (item.CartItem_Users.Sum(ciu => (int?)ciu.Amount) ?? 0) < item.Amount ||
                                     item.CartItem_Users.Any(ciu => ciu.UserId == userId)
                               select new CartItemModel
                               {
                                   Amount = item.Amount,
                                   CartId = item.CartId,
                                   Description = item.Product.Description,
                                   Id = item.Id,
                                   Price = item.Product.Price,
                                   ProductId = item.ProductId,
                                   SelectedAmount = item.CartItem_Users.Sum(ciu => (int?)ciu.Amount) ?? 0,
                                   UserAmount = userId != null &&
                                                item.CartItem_Users.Any(ciu => ciu.UserId == userId) ?
                                                    item.CartItem_Users.Where(ciu => ciu.UserId == userId).Sum(ciu => ciu.Amount) :
                                                    0,
                                   Users = from ciu in item.CartItem_Users
                                           select new CartItemUserModel
                                           {
                                               Amount = ciu.Amount,
                                               Phonenumber = ciu.User.Phonenumber
                                           },
                                   Status = item.CartItem_Users.Count > 1 ? item.CartItem_Users.Select(riu => riu.Status).Max() : item.Status,
                               },
                       MerchantId = cart.MerchantId,
                       MerchantName = cart.Merchant.Name,
                       Date = cart.Created,
                       TotalPrice = cart.CartItems.Sum(x => (float?)(x.Amount * x.Product.Price)) ?? 0
                   };
        }
    }
}