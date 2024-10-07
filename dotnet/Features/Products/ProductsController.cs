using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using dotnet.Services;
using dotnet.Models;
using MongoDB.Bson.Serialization.Attributes;
using MediatR;
using MongoDB.Driver.Search;
using dotnet.Features.Products.Operations;
using Ardalis.Result.AspNetCore;
using Ardalis.Result;

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

    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Invalid)]
    [HttpGet("{Limit}")]
    public async Task<IActionResult> SearchProducts(SearchProducts request)
    {
        var result = await _sender.Send(request);
        return Ok(result);
    }

    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Invalid)]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProduct request)
    {
        var result = await _sender.Send(request);
        return Ok(result);
    }
}
