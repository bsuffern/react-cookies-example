using dotnet.Models;
using dotnet.Services;
using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnet.Features.Products.Operations;

public class CreateProduct : IRequest<CreateProductResponse>
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

    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; } = null!;
}

public class CreateProductHandler : IRequestHandler<CreateProduct, CreateProductResponse>
{
    private readonly ProductsService _productsService;

    public CreateProductHandler(ProductsService productsService)
    {
        _productsService = productsService;
    }

    public async Task<CreateProductResponse> Handle(CreateProduct request, CancellationToken cancellationToken)
    {
        // Check if request is invalid
        if (request == null ||
            request.Price == 0 ||
            request.Name == null ||
            request.Description == null ||
            request.ImageSrc == null)
        {
            return new CreateProductResponse { Success = false, ErrorMessage = "Invalid Request!" };
        }

        // Create product object
        Product product = new()
        {
            Description = request.Description,
            Name = request.Name,
            ImageSrc = request.ImageSrc,
            Price = request.Price
        };

        await _productsService.Create(product);

        return new CreateProductResponse { Success = true, ProductId = product.Id! };
    }
}
