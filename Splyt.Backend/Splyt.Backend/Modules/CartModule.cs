using Backend.Services;
using Nancy;

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
            Post["/"] = _ => PostCart();

            Get["/{cartId:int}"] = p => GetCart(p.cartId);

            Post["/{cartId:int}/Item/{productId:int}"] = p => PostCartItem(p.cartId, p.productId);
            Delete["/Item/{itemId:int}"] = p => DeleteCartItem(p.itemId);

            Post["/Item/{itemId:int}/User/{phonenumber:long}"] = p => PostUser(p.itemId, p.phonenumber);
            Delete["/Item/{itemId:int}/User/{phonenumber:long}"] = p => DeleteUser(p.itemId, p.phonenumber);
            Put["/Item/{itemId:int}/User/{phonenumber:long}/{amount:int}"] = p => PutUser(p.itemId, p.phonenumber, p.amount);

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

            return _cartService.DeleteItem(itemId) ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
        }

        private object DeleteUser(int itemId, long phonenumber)
        {
            return _cartService.RemoveUserFromItem(itemId, phonenumber) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
        }

        private object GetCart(int cartId)
        {
            return Response.AsJson(_cartService.GetCart(cartId));
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

            return Response.AsJson(new { CartId = _cartService.CreateCart(MerchantId) });
        }

        private object PostCartItem(int cartId, int productId)
        {
            if (MerchantId <= 0)
            {
                return HttpStatusCode.Unauthorized;
            }
            if (cartId <= 0)
            {
                return HttpStatusCode.BadRequest;
            }

            return Response.AsJson(new { ItemId = _cartService.CreateItem(cartId, productId) });
        }

        private object PostPayPaymit(int cartId)
        {
            // create a payment from the currently logged in user to the merchant
            if (UserId <= 0)
            {
                return HttpStatusCode.Unauthorized;
            }

            return _cartService.PayCart(cartId, UserId);
        }

        private object PostUser(int itemId, long phonenumber)
        {
            return _cartService.AddUserToItem(itemId, phonenumber) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
        }

        private object PutUser(int itemId, long phonenumber, int amount)
        {
            return _cartService.SetItemAmount(itemId, phonenumber, amount);
        }
    }
}