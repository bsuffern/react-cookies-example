using MediatR;
using Microsoft.AspNetCore.Mvc;
using dotnet.Models;
using dotnet.Services;
using FluentValidation;
using Ardalis.Result;
using Amazon.Runtime.Internal;

namespace dotnet.Features.Carts.Operations;

public class AddItemToNewCart : IRequest<Result<AddItemToNewCartResponse>>
{
    [FromBody]
    public AddItemToNewCartBody Body { get; set; } = null!;
}

public class AddItemToNewCartBody
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
    public string? CartId { get; set; }
}

public class AddItemToNewCartValidator : AbstractValidator<AddItemToNewCart>
{
    private readonly ProductsService _productsService;

    public AddItemToNewCartValidator(ProductsService productsService)
    {
        _productsService = productsService;

        RuleFor(x => x).NotNull().WithMessage("Invalid Request!"); // null request
        RuleFor(x => x.Body.ProductId).NotEmpty().Matches("^[a-fA-F0-9]{24}$").DependentRules(() =>
        {
            RuleFor(x => x.Body.ProductId).MustAsync(async (productId, cancellation) =>
            {
                bool exists = await _productsService.Get(productId) != null;
                return exists;
            }).WithMessage("No product found!").WithErrorCode("404"); // Product exists in DB
        }).WithMessage("No product found!").WithErrorCode("404"); // Valid ObjectID passed
        RuleFor(x => x.Body.Quantity).GreaterThan(0).WithMessage("Invalid Request!"); // Quantity > 0
    }
}

public class AddItemToNewCartHandler : IRequestHandler<AddItemToNewCart, Result<AddItemToNewCartResponse>>
{
    private readonly CartsService _cartsService;

    public AddItemToNewCartHandler(CartsService cartsService)
    {
        _cartsService = cartsService;
    }

    public async Task<Result<AddItemToNewCartResponse>> Handle(AddItemToNewCart request, CancellationToken cancellationToken)
    {
        Cart newCart = new()
        {
            Products =
            [
                new() { ProductId = request.Body.ProductId, Quantity = request.Body.Quantity }
            ]
        };

        // Create new cart and return the cartId
        await _cartsService.Create(newCart);

        return Result<AddItemToNewCartResponse>.Success(new AddItemToNewCartResponse { Success = true, CartId = newCart.Id }); 
    }
}