using MediatR;
using Microsoft.AspNetCore.Mvc;
using dotnet.Models;
using dotnet.Services;
using FluentValidation;
using Amazon.Runtime.Internal;
using MongoDB.Driver;
using Ardalis.Result;

namespace dotnet.Features.Carts.Operations;

public class DeleteItemFromCart : IRequest<Result<DeleteItemFromCartResponse>>
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

public class DeleteItemFromCartValidator : AbstractValidator<DeleteItemFromCart>
{
    private readonly CartsService _cartsService;

    public DeleteItemFromCartValidator(CartsService cartsService)
    {
        _cartsService = cartsService;

        RuleFor(x => x).NotNull().WithMessage("Invalid Request!"); // null request
        RuleFor(x => x.ProductId).NotEmpty().Matches("^[a-fA-F0-9]{24}$").WithMessage("No product found!").WithErrorCode("404"); // product ObjectID valid
        RuleFor(x => x.CartId).NotEmpty().Matches("^[a-fA-F0-9]{24}$").DependentRules(() => {
            RuleFor(x => x).MustAsync(async (request, cancellation) =>
            {
                var cart = await _cartsService.Get(request.CartId);
                if (cart == null)
                    return false;
                if (cart.Products == null)
                    return false;
                bool productExists = cart.Products.SingleOrDefault(x => x.ProductId == request.ProductId) != null;
                return productExists;
            }).WithMessage("No product found!").WithErrorCode("404"); // cart and product exists in cart in DB
        }).WithMessage("No product found!").WithErrorCode("404"); // cart ObjectID valid
    }

    private bool CartExists(Task<Cart> cart)
    {
        if (cart == null)
        {
            return false;
        }

        return true;
    }
}

public class DeleteItemFromCartHandler : IRequestHandler<DeleteItemFromCart, Result<DeleteItemFromCartResponse>>
{
    private readonly CartsService _cartsService;
    private readonly ProductsService _productsService;

    public DeleteItemFromCartHandler(CartsService cartsService, ProductsService productsService)
    {
        _cartsService = cartsService;
        _productsService = productsService;
    }

    public async Task<Result<DeleteItemFromCartResponse>> Handle(DeleteItemFromCart request, CancellationToken cancellationToken)
    {
        // Check if we can find cart
        var cart = await _cartsService.Get(request.CartId);
        var products = cart!.Products;

        // Find product and delete product
        var product = products!.SingleOrDefault(x => x.ProductId == request.ProductId);

        products!.Remove(product!);

        Cart newCart = new()
        {
            Id = request.CartId,
            Products = products
        };
        await _cartsService.Update(request.CartId, newCart);
        Product? deletedProduct = await _productsService.Get(request.ProductId);

        return Result<DeleteItemFromCartResponse>.Success(new DeleteItemFromCartResponse { Success = true, DeletedProduct = deletedProduct });
    }
}