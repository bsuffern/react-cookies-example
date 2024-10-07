using MediatR;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;
using dotnet.Models;
using dotnet.Services;
using FluentValidation;
using Amazon.Runtime.Internal;
using System.Text.RegularExpressions;
using Ardalis.Result;

namespace dotnet.Features.Carts.Operations;

public class UpdateItemQuantity : IRequest<Result<UpdateItemQuantityResponse>>
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

public class UpdateItemQuantityValidator : AbstractValidator<UpdateItemQuantity>
{
    private readonly CartsService _cartsService;
    private readonly ProductsService _productsService;

    public UpdateItemQuantityValidator(CartsService cartsService, ProductsService productsService)
    {
        _cartsService = cartsService;
        _productsService = productsService;

        RuleFor(x => x).NotNull().WithMessage("Invalid Request!"); // null request
        RuleFor(x => x.CartId).NotEmpty().Matches("^[a-fA-F0-9]{24}$").DependentRules(() =>
        {
            RuleFor(x => x).MustAsync(async (request, cancellation) =>
            {
                var cart = await _cartsService.Get(request.CartId);
                if (cart == null)
                    return false;
                var product = cart!.Products!.SingleOrDefault(x => x.ProductId == request.Body.ProductId);
                if (product == null && !request.Body.IncreaseQuantity)
                    return false;
                return true;
            }).WithMessage("No cart found!").WithErrorCode("404"); // cart exists in DB
            RuleFor(x => x.Body.ProductId).Matches("^[a-fA-F0-9]{24}$").DependentRules(() =>
            {
                RuleFor(x => x.Body.ProductId).MustAsync(async (id, cancellation) =>
                {
                    bool productExists = await _productsService.Get(id) != null;
                    return productExists;
                }).WithMessage("No product found!").WithErrorCode("404"); // product exists in DB
            }).WithMessage("Invalid Request!"); // product ObjectID valid
        }).WithMessage("Invalid Request!"); // cart ObjectID valid
    }
}

public class UpdateItemQuantityHandler : IRequestHandler<UpdateItemQuantity, Result<UpdateItemQuantityResponse>>
{
    private readonly CartsService _cartsService;
    private readonly ProductsService _productsService;

    public UpdateItemQuantityHandler(CartsService cartsService, ProductsService productsService)
    {
        _cartsService = cartsService;
        _productsService = productsService;
    }

    public async Task<Result<UpdateItemQuantityResponse>> Handle(UpdateItemQuantity request, CancellationToken cancellationToken)
    {
        var cart = await _cartsService.Get(request.CartId);

        // Check if product exists in our database
        var checkProduct = await _productsService.Get(request.Body.ProductId);
        var products = cart!.Products;

        // If products is null
        if (products == null)
        {
            Cart noProductCart = new()
            {
                Id = request.CartId,
                Products = [
                    new()
                    {
                        ProductId = request.Body.ProductId,
                        Quantity = 1
                    }
                ]
            };

            await _cartsService.Update(request.CartId, noProductCart);

            return Result<UpdateItemQuantityResponse>.Success(new UpdateItemQuantityResponse { Success = true, Cart = noProductCart });
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

                return Result<UpdateItemQuantityResponse>.Success(new UpdateItemQuantityResponse { Success = true, Cart = addedProductCart } );
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

        return Result<UpdateItemQuantityResponse>.Success( new UpdateItemQuantityResponse { Success = true, Cart = newCart } );
    }
}