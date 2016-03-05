﻿using Backend.Models;
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
            Post["/"] = _ => PostCart();

            Get["/{cartId:int}"] = p => GetCart(p.cartId);
            Get["/{cartId:int}/User/{phonenumber:long}"] = p => GetCart(p.cartId, p.phonenumber);

            Post["/{cartId:int}/Item"] = p => PostCartItem(p.cartId);
            Post["/{cartId:int}/Item/{productId:int}"] = p => PostCartItem(p.cartId, p.productId);
            Delete["/Item/{itemId:int}"] = p => DeleteCartItem(p.itemId);

            Post["/Item/{itemId:int}/User/{phonenumber:long}"] = p => PostUser(p.itemId, p.phonenumber);
            Delete["/Item/{itemId:int}/User/{phonenumber:long}"] = p => DeleteUser(p.itemId, p.phonenumber);
            Put["/Item/{itemId:int}/User/{phonenumber:long}/{amount:int}"] = p => PutUser(p.itemId, p.phonenumber, p.amount);

            Post["/{cartId:int}/PayPaymit"] = p => PostPayPaymit(p.cartId);
        }

        private object DeleteCartItem(int itemId)
        {
            _cartService.DeleteItem(itemId);
            return HttpStatusCode.OK;
        }

        private object DeleteUser(int itemId, long phonenumber)
        {
            _cartService.RemoveUserFromItem(itemId, phonenumber);
            return HttpStatusCode.OK;
        }

        private object GetCart(int cartId)
        {
            return Response.AsJson(_cartService.GetCart(cartId));
        }

        private object GetCart(int cartId, long phonenumber)
        {
            return Response.AsJson(_cartService.GetCart(cartId, phonenumber));
        }

        private object GetCarts()
        {
            int merchantId = Request.Query.merchantId;
            return _cartService.GetCarts(merchantId);
        }

        private Response OnBeforeRequest(NancyContext arg)
        {
            //UserId = arg.Request.Query.userId;
            //MerchantId = arg.Request.Query.merchantId;

            //if (UserId <= 0 && MerchantId <= 0)
            //{
            //    return HttpStatusCode.Unauthorized;
            //}
            //return null;
            return null;
        }

        private object PostCart()
        {
            int merchantId = Request.Query.merchantId;
            return Response.AsJson(new { CartId = _cartService.CreateCart(merchantId) });
        }

        private object PostCartItem(int cartId)
        {
            var model = this.Bind<CartItemModel>();
            return Response.AsJson(new { ItemId = _cartService.CreateItem(cartId, model.Description, model.Price, model.Amount) });
        }

        private object PostCartItem(int cartId, int productId)
        {
            return Response.AsJson(new { ItemId = _cartService.CreateItem(cartId, productId, 1) });
        }

        private object PostPayPaymit(int cartId)
        {
            // create a payment from the currently logged in user to the merchant
            const int userId = 0;
            return _cartService.PayCart(cartId, userId);
        }

        private object PostUser(int itemId, long phonenumber)
        {
            _cartService.AddUserToItem(itemId, phonenumber);
            return HttpStatusCode.OK;
        }

        private object PutUser(int itemId, long phonenumber, int amount)
        {
            _cartService.SetItemAmount(itemId, phonenumber, amount);
            return HttpStatusCode.OK;
        }
    }
}