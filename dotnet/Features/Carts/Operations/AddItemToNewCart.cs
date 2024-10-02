using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using dotnet.Models;
using dotnet.Services;

namespace dotnet.Features.Carts.Operations;

public class AddItemToNewCart : IRequest<AddItemToNewCartResponse>
{
    [FromBody]
    public string ProductId { get; set; } = null!;
    [FromBody]
    public int Quantity { get; set; }
}

public class AddItemToNewCartResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? CartId { get; set; }
}

public class AddItemToNewCartHandler : IRequestHandler<AddItemToNewCart, AddItemToNewCartResponse>
{
    private readonly CartsService _cartsService;
    private readonly ProductsService _productsService;

    public AddItemToNewCartHandler(CartsService cartsService, ProductsService productsService)
    {
        _cartsService = cartsService;
        _productsService = productsService;
    }

    public async Task<AddItemToNewCartResponse> Handle(AddItemToNewCart request, CancellationToken cancellationToken)
    {
        // Check if request is invalid
        if (request == null ||
            request.ProductId == null)
        {
            return new AddItemToNewCartResponse { Success = false, ErrorMessage = "Invalid Request!" };
        }

        CartItem newCartItem = new()
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        Cart newCart = new()
        {
            Products = new List<CartItem> { newCartItem }
        };

        // Check if product exists in our database

        var product = await _productsService.Get(request.ProductId);

        if (product == null)
        {
            return new AddItemToNewCartResponse { Success = false, ErrorMessage = "No product found!" };
        }

        // Create new cart and return the cartId
        await _cartsService.Create(newCart);

        return new AddItemToNewCartResponse { Success = true, CartId = newCart.Id };
    }
}