using MediatR;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;
using dotnet.Models;
using dotnet.Services;

namespace dotnet.Features.Carts.Operations;

public class UpdateItemQuantity : IRequest<UpdateItemQuantityResponse>
{
    [FromRoute]
    public string CartId { get; set; } = null!;
    [FromBody]
    public UpdateItemQuantityBody Body { get; set; } = null!;
}

public class UpdateItemQuantityBody
{
    [FromBody]
    public string ProductId { get; set; } = null!;
    [FromBody]
    public bool IncreaseQuantity { get; set; }
}

public class UpdateItemQuantityResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Cart? Cart { get; set; }
}

public class UpdateItemQuantityHandler : IRequestHandler<UpdateItemQuantity, UpdateItemQuantityResponse>
{
    private readonly CartsService _cartsService;
    private readonly ProductsService _productsService;

    public UpdateItemQuantityHandler(CartsService cartsService, ProductsService productsService)
    {
        _cartsService = cartsService;
        _productsService = productsService;
    }

    public async Task<UpdateItemQuantityResponse> Handle(UpdateItemQuantity request, CancellationToken cancellationToken)
    {
        // Check if cartID is null or empty
        if (request.CartId == null ||
            request.CartId == "" ||
            request == null)
        {
            return new UpdateItemQuantityResponse { Success = false, ErrorMessage = "Invalid request!" };
        }

        var cart = await _cartsService.Get(request.CartId);

        // Check if cart doesn't exists
        if (cart == null)
        {
            return new UpdateItemQuantityResponse { Success = false, ErrorMessage = "No cart found!" };
        }

        // Check if product exists in our database

        var checkProduct = await _productsService.Get(request.Body.ProductId);

        if (checkProduct == null)
        {
            return new UpdateItemQuantityResponse { Success = false, ErrorMessage = "No product found!" };
        }


        var products = cart.Products;

        // If products is null
        if (products == null)
        {
            CartItem newProduct = new()
            {
                ProductId = request.Body.ProductId,
                Quantity = 1
            };

            Cart noProductCart = new()
            {
                Id = request.CartId,
                Products = [newProduct]
            };

            await _cartsService.Update(request.CartId, noProductCart);

            return new UpdateItemQuantityResponse { Success = true, Cart = noProductCart };
        }

        // Find product
        var product = products.SingleOrDefault(x => x.ProductId == request.Body.ProductId);

        if (product == null)
        {
            // No product found hence we create the entry if increaseQuantity > 0
            if (request.Body.IncreaseQuantity)
            {
                products.Add(new CartItem { ProductId = request.Body.ProductId, Quantity = 1 });

                Cart addedProductCart = new() { Id = request.CartId, Products = products };

                await _cartsService.Update(request.CartId, addedProductCart);

                return new UpdateItemQuantityResponse { Success = true, Cart = addedProductCart };
            }
            else // Quantity is negative and product doesn't exist
            {
                return new UpdateItemQuantityResponse { Success = false, ErrorMessage = "Invalid request!" };
            }
        }

        var newQuantity = product!.Quantity + (request.Body.IncreaseQuantity ? 1 : -1);

        if (newQuantity == 0)
        {
            products.Remove(product);
        }
        else
        {
            var index = products.FindIndex(x => x.ProductId == request.Body.ProductId);

            products[index] = new CartItem
            {
                ProductId = request.Body.ProductId,
                Quantity = newQuantity
            };
        }

        Cart newCart = new()
        {
            Id = request.CartId,
            Products = products
        };

        await _cartsService.Update(request.CartId, newCart);

        return new UpdateItemQuantityResponse { Success = true, Cart = newCart };
    }
}