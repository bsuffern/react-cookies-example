using Amazon.Runtime.Internal;
using dotnet.Models;
using dotnet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MediatR;
using dotnet.Features.Carts.Operations;

namespace dotnet.Features.Carts;

[Route("api/[controller]")]
[ApiController]
public class CartsController : ControllerBase
{
    private readonly ISender _sender;

    public CartsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{CartId}")]
    public async Task<IActionResult> GetCartById(GetCartById request)
    {
        var result = await _sender.Send(request);

        if (result.Success == false)
        {
            if (result.ErrorMessage == "No cart found!")
            {
                return NotFound(result.ErrorMessage);
            }
            else if (result.ErrorMessage == "Invalid Request!")
            {
                return BadRequest(result.ErrorMessage);
            }
        }

        return Ok(result.Cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddItemToNewCart(AddItemToNewCart request)
    {
        var result = await _sender.Send(request);

        if (result.Success == false)
        {
            if (result.ErrorMessage == "No product found!")
            {
                return NotFound(result.ErrorMessage);
            }
            else if (result.ErrorMessage == "Invalid Request!")
            {
                return BadRequest(result.ErrorMessage);
            }
        }

        return Ok(result.CartId);
    }

    [HttpPut("{CartId}")]
    public async Task<IActionResult> UpdateItemQuantity(UpdateItemQuantity request)
    {
        var result = await _sender.Send(request);

        if (result.Success == false)
        {
            if (result.ErrorMessage == "No product found!" || result.ErrorMessage == "No cart found!")
            {
                return NotFound(result.ErrorMessage);
            }
            else if (result.ErrorMessage == "Invalid Request!")
            {
                return BadRequest(result.ErrorMessage);
            }
        }

        return Ok(result.Cart);
    }

    [HttpDelete("{CartId}/{ProductId}")]
    public async Task<IActionResult> DeleteItemFromCart(DeleteItemFromCart request)
    {
        var result = await _sender.Send(request);

        if (result.Success == false)
        {
            if (result.ErrorMessage == "No product found!" || result.ErrorMessage == "No cart found!")
            {
                return NotFound(result.ErrorMessage);
            }
            else if (result.ErrorMessage == "Invalid Request!")
            {
                return BadRequest(result.ErrorMessage);
            }
        }

        return Ok(result.DeletedProduct);
    }
}
