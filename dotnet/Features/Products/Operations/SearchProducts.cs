using Ardalis.Result;
using dotnet.Models;
using dotnet.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dotnet.Features.Products.Operations;

public class SearchProducts : IRequest<Result<SearchProductsResponse>>
{
    [FromRoute]
    public int Limit { get; set; }
}

public class SearchProductsResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<Product>? Products { get; set; }
}

public class SearchProductsValdiator : AbstractValidator<SearchProducts>
{
    public SearchProductsValdiator()
    {
        RuleFor(x => x).NotNull().WithMessage("Invalid Request!"); // null request
        RuleFor(x => x.Limit).NotEqual(0).WithMessage("Invalid Request!"); // limit supplied
    }
}

public class SearchProductsHandler : IRequestHandler<SearchProducts, Result<SearchProductsResponse>>
{
    private readonly ProductsService _productsService;

    public SearchProductsHandler(ProductsService productsService)
    {
        _productsService = productsService;
    }

    public async Task<Result<SearchProductsResponse>> Handle(SearchProducts request, CancellationToken cancellationToken)
    {
        var products = await _productsService.Search(request.Limit);
        return Result<SearchProductsResponse>.Success(new SearchProductsResponse { Success = true, Products = products });
    }
}