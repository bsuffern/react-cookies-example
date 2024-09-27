using dotnet.Models;
using dotnet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly CartsService _cartsService;

    public CartController(CartsService cartsService)
    {
        _cartsService = cartsService;
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

        return Ok(cart);
    }

    [HttpPost]
    public async Task<IActionResult> PostCart([FromBody]Cart newCart)
    {
        // Check if request is invalid
        if (newCart == null)
        {
            return BadRequest("Invalid request!");
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

        var products = cart.Products;

        // Find product
        var product = products.SingleOrDefault(x => x.ProductId == request.ProductId);
        
        if (product == null)
        {
            return NotFound("No product found!");
        }

        var newQuantity = product.Quantity + (request.IncreaseQuantity ? 1 : -1);

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

        return Ok(cart);
    }
}
