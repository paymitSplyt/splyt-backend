using Backend.DataAccess.Models;
using Backend.Services;
using Nancy;
using Nancy.ModelBinding;

namespace Backend.Modules
{
    public class CartModule : Module
    {
        private readonly CartService _cartService;

        public CartModule(CartService cartService)
            : base("/Cart")
        {
            Before += OnBeforeRequest;

            _cartService = cartService;
            Get["/"] = _ => GetCarts();

            Get["/{cartId:int}"] = p => GetCart(p.cartId);

            // Expects query parameter "merchantId"
            Post["/"] = _ => PostCart();

            Post["/{cartId:int}/Item"] = p => PostCartItem(p.cartId);
            Delete["/Item/{itemId:int}"] = p => DeleteCartItem(p.itemId);

            Post["/Item/{itemId:int}/User/{phonenumber:long}"] = p => PostUser(p.itemId, p.phonenumber);
            Delete["/Item/{itemId:int}/User/{phonenumber:long}"] = p => DeleteUser(p.itemId, p.phonenumber);

            Post["/{cartId:int}/PayPaymit"] = p => PostPayPaymit(p.cartId);
        }

        /// <summary>
        /// Will come from authentication.
        /// </summary>
        public int MerchantId { get; set; }

        /// <summary>
        /// Will come from authentication.
        /// </summary>
        public int UserId { get; private set; }

        private object DeleteCartItem(int itemId)
        {
            if (MerchantId <= 0)
            {
                return HttpStatusCode.Unauthorized;
            }

            using (var service = new CartService())
            {
                return service.DeleteItem(itemId) ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            }
        }

        private object DeleteUser(int itemId, long phonenumber)
        {
            using (var service = new CartService())
            {
                return service.RemoveUserFromItem(itemId, phonenumber) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            }
        }

        private object GetCart(int cartId)
        {
            using (var service = new CartService())
            {
                return Response.AsJson(service.GetCart(cartId));
            }
        }

        private object GetCarts()
        {
            if (MerchantId <= 0)
            {
                return HttpStatusCode.Unauthorized;
            }

            return _cartService.GetCarts(MerchantId);
        }

        private Response OnBeforeRequest(NancyContext arg)
        {
            UserId = arg.Request.Query.userId;
            MerchantId = arg.Request.Query.merchantId;

            if (UserId <= 0 && MerchantId <= 0)
            {
                return HttpStatusCode.Unauthorized;
            }
            return null;
        }

        private object PostCart()
        {
            if (MerchantId <= 0)
            {
                return HttpStatusCode.Unauthorized;
            }

            using (var service = new CartService())
            {
                return Response.AsJson(new { CartId = service.CreateCart(MerchantId) });
            }
        }

        private object PostCartItem(int cartId)
        {
            if (MerchantId <= 0)
            {
                return HttpStatusCode.Unauthorized;
            }
            if (cartId <= 0)
            {
                return HttpStatusCode.BadRequest;
            }

            var item = this.Bind<CartItem>();

            item.CartId = cartId;
            item.Status = PaymentStatus.Open;

            using (var service = new CartService())
            {
                return Response.AsJson(new { ItemId = service.CreateItem(item) });
            }
        }

        private object PostPayPaymit(int cartId)
        {
            // create a payment from the currently logged in user to the merchant
            if (UserId <= 0)
            {
                return HttpStatusCode.Unauthorized;
            }

            using (var service = new CartService())
            {
                return service.PayCart(cartId, UserId);
            }
        }

        private object PostUser(int itemId, long phonenumber)
        {
            using (var service = new CartService())
            {
                return service.AddUserToItem(itemId, phonenumber) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            }
        }
    }
}