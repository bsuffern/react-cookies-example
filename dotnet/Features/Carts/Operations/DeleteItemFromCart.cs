using MediatR;
using Microsoft.AspNetCore.Mvc;
using dotnet.Models;
using dotnet.Services;

namespace dotnet.Features.Carts.Operations;

public class DeleteItemFromCart : IRequest<DeleteItemFromCartResponse>
{
    [FromRoute]
    public string CartId { get; set; } = null!;
    [FromRoute]
    public string ProductId { get; set; } = null!;
}

public class DeleteItemFromCartResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Product? DeletedProduct { get; set; }
}

public class DeleteItemFromCartHandler : IRequestHandler<DeleteItemFromCart, DeleteItemFromCartResponse>
{
    private readonly CartsService _cartsService;
    private readonly ProductsService _productsService;

    public DeleteItemFromCartHandler(CartsService cartsService, ProductsService productsService)
    {
        _cartsService = cartsService;
        _productsService = productsService;
    }

    public async Task<DeleteItemFromCartResponse> Handle(DeleteItemFromCart request, CancellationToken cancellationToken)
    {
        // Check if invalid request
        if (request.CartId == null || request.ProductId == null)
        {
            return new DeleteItemFromCartResponse { Success = false, ErrorMessage = "Invalid request!" };
        }

        // Check if we can find cart
        var cart = await _cartsService.Get(request.CartId);

        if (cart == null)
        {
            return new DeleteItemFromCartResponse { Success = false, ErrorMessage = "No cart found!" };
        }

        var products = cart.Products;

        if (products == null)
        {
            return new DeleteItemFromCartResponse { Success = false, ErrorMessage = "No product found!" };
        }

        // Find product and delete product
        var product = products.SingleOrDefault(x => x.ProductId == request.ProductId);

        if (product == null)
        {
            return new DeleteItemFromCartResponse { Success = false, ErrorMessage = "No product found!" };
        }

        products.Remove(product);

        Cart newCart = new()
        {
            Id = request.CartId,
            Products = products
        };
        await _cartsService.Update(request.CartId, newCart);

        Product? deletedProduct = await _productsService.Get(request.ProductId);

        return new DeleteItemFromCartResponse { Success = true, DeletedProduct = deletedProduct };
    }
}