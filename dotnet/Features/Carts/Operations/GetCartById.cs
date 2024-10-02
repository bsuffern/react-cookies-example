using dotnet.Models;
using dotnet.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dotnet.Features.Carts.Operations;

public class GetCartById : IRequest<GetCartByIdResponse>
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

public class GetCartByIdHandler : IRequestHandler<GetCartById, GetCartByIdResponse>
{
    private readonly CartsService _cartsService;
    private readonly ProductsService _productsService;

    public GetCartByIdHandler(CartsService cartsService, ProductsService productsService)
    {
        _cartsService = cartsService;
        _productsService = productsService;
    }

    public async Task<GetCartByIdResponse> Handle(GetCartById request, CancellationToken cancellationToken)
    {
        // Check request is invalid
        if (request.CartId == null || request.CartId == "")
        {
            return new GetCartByIdResponse { Success = false, ErrorMessage = "Invalid request!" };
        }

        var cart = await _cartsService.Get(request.CartId);

        // Check if cart valid
        if (cart == null)
        {
            return new GetCartByIdResponse { Success = false, ErrorMessage = "No cart found!" };
        }

        // Loop over cart items and create GetCartResponse
        List<CartDisplay> response = new List<CartDisplay>();

        foreach (var item in cart.Products!)
        {
            var product = await _productsService.Get(item.ProductId);

            response.Add(new CartDisplay { Quantity = item.Quantity, Product = product! });
        }

        return new GetCartByIdResponse { Success = true, Cart = response };
    }
}