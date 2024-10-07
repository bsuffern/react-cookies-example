using Amazon.Runtime.Internal;
using dotnet.Models;
using dotnet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MediatR;
using dotnet.Features.Carts.Operations;
using Ardalis.Result.AspNetCore;
using Ardalis.Result;

namespace dotnet.Features.Carts;

[Route("api/[controller]")]
[ApiController]
public class CartsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Invalid)]
    [HttpGet("{CartId}")]
    public async Task<IActionResult> GetCartById(GetCartById request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Invalid)]
    [HttpPost]
    public async Task<IActionResult> AddItemToNewCart(AddItemToNewCart request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Invalid)]
    [HttpPut("{CartId}")]
    public async Task<IActionResult> UpdateItemQuantity(UpdateItemQuantity request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.NotFound, ResultStatus.Invalid)]
    [HttpDelete("{CartId}/{ProductId}")]
    public async Task<IActionResult> DeleteItemFromCart(DeleteItemFromCart request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}
