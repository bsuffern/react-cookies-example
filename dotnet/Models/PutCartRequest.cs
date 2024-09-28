namespace dotnet.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class PutCartRequest
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; } = null!;
    public bool IncreaseQuantity { get; set; }
}
