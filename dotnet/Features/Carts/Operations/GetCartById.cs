using Ardalis.Result;
using dotnet.Models;
using dotnet.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dotnet.Features.Carts.Operations;

public class GetCartById : IRequest<Result<GetCartByIdResponse>>
{
    [FromRoute]
    public string CartId { get; set; } = null!;
}

public class GetCartByIdResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<CartDisplay>? Cart { get; set; }
}

public class GetCartByIdValidator : AbstractValidator<GetCartById>
{
    private readonly CartsService _cartsService;

    public GetCartByIdValidator(CartsService cartsService)
    {
        _cartsService = cartsService;

        RuleFor(x => x).NotNull().WithMessage("Invalid Request!"); // null request
        RuleFor(x => x.CartId).NotEmpty().Matches("^[a-fA-F0-9]{24}$").DependentRules(() =>
        {
            RuleFor(x => x.CartId).MustAsync(async (cartId, cancellation) =>
            {
                return await _cartsService.Get(cartId) != null;
            }).WithMessage("No cart found!").WithErrorCode("404"); // cart exists in DB
        }).WithMessage("No product found!").WithErrorCode("404"); // cart ObjectID valid
    }
}

public class GetCartByIdHandler : IRequestHandler<GetCartById, Result<GetCartByIdResponse>>
{
    private readonly CartsService _cartsService;
    private readonly ProductsService _productsService;

    public GetCartByIdHandler(CartsService cartsService, ProductsService productsService)
    {
        _cartsService = cartsService;
        _productsService = productsService;
    }

    public async Task<Result<GetCartByIdResponse>> Handle(GetCartById request, CancellationToken cancellationToken)
    {
        var cart = await _cartsService.Get(request.CartId);

        // Loop over cart items and create GetCartResponse
        List<CartDisplay> response = new List<CartDisplay>();

        foreach (var item in cart!.Products!)
        {
            var product = await _productsService.Get(item.ProductId);
            response.Add(new CartDisplay { Quantity = item.Quantity, Product = product! });
        }

        return Result<GetCartByIdResponse>.Success( new GetCartByIdResponse { Success = true, Cart = response } );
    }
}