using Amazon.Runtime.Internal;
using dotnet.Models;
using dotnet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartsController : ControllerBase
{
    private readonly CartsService _cartsService;
    private readonly ProductsService _productsService;

    public CartsController(CartsService cartsService, ProductsService productsService)
    {
        _cartsService = cartsService;
        _productsService = productsService;
    }

    [HttpGet("{cartId}")]
    public async Task<IActionResult> GetCart([FromRoute]string cartId)
    {
        // Check if cartID is null or empty
        if (cartId == null || cartId == "")
        {
            return BadRequest("Invalid request!");
        }

        var cart = await _cartsService.GetAsync(cartId);

        // Check if cart valid
        if (cart == null)
        { 
            return NotFound("No cart found!");
        }

        // Loop over cart items and create GetCartResponse
        List<GetCartResponse> response = new List<GetCartResponse>();

        foreach (var item in cart.Products!)
        {
            var product = await _productsService.GetAsync(item.ProductId);

            response.Add(new GetCartResponse { Quantity = item.Quantity, Product = product! });
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> PostCart([FromBody]Cart newCart)
    {
        // Check if request is invalid
        if (newCart.Id != null ||
            newCart.Products == null ||
            newCart == null)
        {
            return BadRequest("Invalid request!");
        }

        // Check if product exists in our database
        
        var product = await _productsService.GetAsync(newCart.Products[0].ProductId);

        if (product == null)
        {
            return NotFound("Product not found!");
        }

        // Create new cart and return the cartId
        await _cartsService.CreateAsync(newCart);

        return Ok(new { id = newCart.Id });
    }

    [HttpPut("{cartId}")]
    public async Task<IActionResult> PutCart([FromRoute]string cartId, [FromBody]PutCartRequest request)
    {
        // Check if cartID is null or empty
        if (cartId == null || cartId == "")
        {
            return BadRequest("Invalid request!");
        }

        // Check if request is invalid
        if (request == null)
        {
            return BadRequest("Invalid request!");
        }

        var cart = await _cartsService.GetAsync(cartId);

        // Check if cart doesn't exists
        if (cart == null)
        {
            return NotFound("No cart found!");
        }

        // Check if product exists in our database

        var checkProduct = await _productsService.GetAsync(request.ProductId);

        if (checkProduct == null)
        {
            return NotFound("Product not found!");
        }


        var products = cart.Products;

        // If products is null
        if (products == null)
        {
            CartProduct newProduct = new()
            {
                ProductId = request.ProductId,
                Quantity = 1
            };

            Cart noProductCart = new()
            {
                Id = cartId,
                Products = [newProduct]
            };

            await _cartsService.UpdateAsync(cartId, noProductCart);

            return Ok(noProductCart);
        }

        // Find product
        var product = products.SingleOrDefault(x => x.ProductId == request.ProductId);
        
        if (product == null)
        {
            // No product found hence we create the entry if increaseQuantity > 0
            if (request.IncreaseQuantity)
            {
                products.Add(new CartProduct { ProductId = request.ProductId, Quantity = 1 });

                Cart addedProductCart = new() { Id = cartId, Products = products };

                await _cartsService.UpdateAsync(cartId, addedProductCart);

                return Ok(addedProductCart);
            } else // Quantity is negative and product doesn't exist
            {
                return BadRequest("Invalid Request!");
            }
        }

        var newQuantity = product!.Quantity + (request.IncreaseQuantity ? 1 : -1);

        if (newQuantity == 0)
        {
            products.Remove(product);
        }
        else
        {
            var index = products.FindIndex(x => x.ProductId == request.ProductId);

            products[index] = new CartProduct {
                ProductId = request.ProductId,
                Quantity = newQuantity
            };
        }

        Cart newCart = new()
        {
            Id = cartId,
            Products = products
        };

        await _cartsService.UpdateAsync(cartId, newCart);

        return Ok(newCart);
    }

    [HttpDelete("{cartId}")]
    public async Task<IActionResult> DeleteProduct([FromRoute]string cartId, [FromBody]string productId)
    {
        // Check if invalid request
        if (cartId == null || productId == null)
        {
            return BadRequest("Invalid request");
        }

        // Check if we can find cart
        var cart = await _cartsService.GetAsync(cartId);

        if (cart == null)
        {
            return NotFound("Cart not found!");
        }

        // Delete product from cart
        var products = cart.Products;

        // Find product
        var product = products.SingleOrDefault(x => x.ProductId == productId);

        if (product == null)
        {
            return NotFound("No product found!");
        }

        products.Remove(product);

        Cart newCart = new()
        {
            Id = cartId,
            Products = products
        };

        await _cartsService.UpdateAsync(cartId, newCart);

        return Ok(newCart);
    }
}
