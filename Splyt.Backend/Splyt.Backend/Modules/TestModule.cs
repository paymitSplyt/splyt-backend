using Backend.DataAccess;
using Backend.DataAccess.Models;
using Nancy;

namespace Backend.Modules
{
    public class TestModule : Module
    {
        public TestModule()
            : base("/Test")
        {
            Get["/CreateTestData"] = _ => CreateTestData();
        }

        private object CreateTestData()
        {
            using (var context = new DataContext())
            {
                var user1 = new User
                {
                    Phonenumber = 41799876543
                };
                context.Users.Add(user1);

                var user2 = new User
                {
                    Phonenumber = 41311234567
                };
                context.Users.Add(user2);

                var merchant = new Merchant
                {
                    Name = "Tramdepot",
                    Alias = "#SIXHAXBEER"
                };
                context.Merchants.Add(merchant);

                var product1 = new Product
                {
                    Description = "Honey Spice IPA",
                    Merchant = merchant,
                    Price = 4f
                };
                context.Products.Add(product1);
                var product2 = new Product
                {
                    Description = "Anker",
                    Merchant = merchant,
                    Price = 0.5f
                };
                context.Products.Add(product2);
                var product3 = new Product
                {
                    Description = "Water Bottle",
                    Merchant = merchant,
                    Price = 0.5f
                };
                context.Products.Add(product3);
                var product4 = new Product
                {
                    Description = "Big Beef Burger",
                    Merchant = merchant,
                    Price = 21.95f
                };
                context.Products.Add(product4);

                var cart = new Cart
                {
                    Merchant = merchant
                };
                context.Requests.Add(cart);

                var item1 = new CartItem
                {
                    Amount = 3,
                    Cart = cart,
                    Product = product1
                };
                context.RequestItems.Add(item1);
                var item2 = new CartItem
                {
                    Amount = 1,
                    Cart = cart,
                    Product = product2
                };
                context.RequestItems.Add(item2);
                var item3 = new CartItem
                {
                    Amount = 1,
                    Cart = cart,
                    Product = product3
                };
                context.RequestItems.Add(item3);
                var item4 = new CartItem
                {
                    Amount = 2,
                    Cart = cart,
                    Product = product4
                };
                context.RequestItems.Add(item4);

                var riu1 = new CartItem_User
                {
                    CartItem = item1,
                    User = user1,
                    Amount = 1
                };
                context.RequestItemUsers.Add(riu1);

                var riu2 = new CartItem_User
                {
                    CartItem = item2,
                    User = user2,
                    Amount = 3
                };
                context.RequestItemUsers.Add(riu2);

                var riu3 = new CartItem_User
                {
                    CartItem = item4,
                    User = user2,
                    Amount = 2
                };
                context.RequestItemUsers.Add(riu3);
                context.SaveChanges();
            }
            return HttpStatusCode.OK;
        }
    }
}