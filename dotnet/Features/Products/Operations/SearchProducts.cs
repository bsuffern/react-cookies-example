using dotnet.Models;
using dotnet.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dotnet.Features.Products.Operations;

public class SearchProducts : IRequest<SearchProductsResponse>
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

public class SearchProductsHandler : IRequestHandler<SearchProducts, SearchProductsResponse>
{
    private readonly ProductsService _productsService;

    public SearchProductsHandler(ProductsService productsService)
    {
        _productsService = productsService;
    }

    public async Task<SearchProductsResponse> Handle(SearchProducts request, CancellationToken cancellationToken)
    {
        // Check if request is invalid
        if (request == null ||
            request.Limit == 0)
        {
            return new SearchProductsResponse { Success = false, ErrorMessage = "Invalid Request!" };
        }

        var products = await _productsService.Search(request.Limit);

        return new SearchProductsResponse { Success = true, Products = products };
    }
}