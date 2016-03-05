# API

* /Cart
 * GET: returns all open Carts for the currently logged in merchant (requires URL parameter “merchantId”
 * POST: create a new Cart for the currently logged in merchant (requires URL parameter “merchantId” and returns { cartId: <cart-id> }
* /Cart/{cartId:int}
 * GET
* /{cartId:int}/Item/{productId:int}
 * POST: Adds a new item/position to the cart. Returns { itemId: <item-id> }
* /Item/{itemId:int}
 * DELETE: Deletes an item from a cart
* /Item/{itemId:int}/User/{phonenumber:long}
* POST: assigns an item to a user
* DELETE: remove the user’s assignation to the item
* /Item/{itemId:int}/User/{phonenumber:long}/Amount/{amount:int}
 * PUT: set the amount of the same product for a user