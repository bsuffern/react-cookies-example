using Ardalis.Result;
using dotnet.Models;
using dotnet.Services;
using FluentValidation;
using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnet.Features.Products.Operations;

public class CreateProduct : IRequest<Result<CreateProductResponse>>
{
    [BsonElement("Name")]
    public string Name { get; set; } = null!;

    [BsonElement("Description")]
    public string Description { get; set; } = null!;

    [BsonElement("Price")]
    public decimal Price { get; set; }

    [BsonElement("ImageSrc")]
    public string ImageSrc { get; set; } = null!;
}

public class CreateProductResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string ProductId { get; set; } = null!;
}

public class CreateProductValdiator : AbstractValidator<CreateProduct>
{
    public CreateProductValdiator()
    {
        RuleFor(x => x).NotNull().WithMessage("Invalid Request!");
        RuleFor(x => x.Price).NotEqual(0).WithMessage("Invalid Request!");
        RuleFor(x => x.Name).NotNull().WithMessage("Invalid Request!");
        RuleFor(x => x.Description).NotNull().WithMessage("Invalid Request!");
        RuleFor(x => x.ImageSrc).NotNull().WithMessage("Invalid Request!"); // Every field is specified
    }
}

public class CreateProductHandler : IRequestHandler<CreateProduct, Result<CreateProductResponse>>
{
    private readonly ProductsService _productsService;

    public CreateProductHandler(ProductsService productsService)
    {
        _productsService = productsService;
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProduct request, CancellationToken cancellationToken)
    {
        // Create product object
        Product product = new()
        {
            Description = request.Description,
            Name = request.Name,
            ImageSrc = request.ImageSrc,
            Price = request.Price
        };

        await _productsService.Create(product);

        return Result<CreateProductResponse>.Success(new CreateProductResponse { Success = true, ProductId = product.Id! });
    }
}
