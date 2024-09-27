using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnet.Models;
public class Cart
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Products")]
    public List<CartProduct>? Products { get; set; }
}