using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using dotnet.Services;
using dotnet.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ProductsService _productsService;

    public ProductsController(ProductsService productsService)
    {
        _productsService = productsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productsService.GetAsync();

        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> PostProduct(Product product)
    {
        await _productsService.CreateAsync(product);

        return Ok(product);
    }
}
