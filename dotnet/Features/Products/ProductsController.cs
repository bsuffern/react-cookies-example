using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using dotnet.Services;
using dotnet.Models;
using MongoDB.Bson.Serialization.Attributes;
using MediatR;
using MongoDB.Driver.Search;
using dotnet.Features.Products.Operations;

namespace dotnet.Features.Products;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{Limit}")]
    public async Task<IActionResult> SearchProducts(SearchProducts request)
    {
        var result = await _sender.Send(request);

        if (result.Success == false)
        {
            if (result.ErrorMessage == "Invalid Request!")
            {
                return BadRequest(result.ErrorMessage);
            }
        }

        return Ok(result.Products);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProduct request)
    {
        var result = await _sender.Send(request);

        if (result.Success == false)
        {
            if (result.ErrorMessage == "Invalid Request!")
            {
                return BadRequest(result.ErrorMessage);
            }
        }

        return Ok(result.ProductId);
    }
}
