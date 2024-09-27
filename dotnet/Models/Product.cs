﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnet.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; } = null!;

    [BsonElement("Description")]
    public string Description { get; set; } = null!;

    [BsonElement("Price")]
    public decimal Price { get; set; }

    [BsonElement("ImageSrc")]
    public string ImageSrc { get; set; } = null!;


}
