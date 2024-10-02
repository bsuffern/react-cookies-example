using dotnet.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace dotnet.Services;

public class ProductsService
{
    readonly IMongoCollection<Product> _productsCollection;

    public ProductsService(IOptions<FashionWebsiteDatabaseSettings> fashionWebsiteDatabaseSettings)
    {
        var mongoClient = new MongoClient(fashionWebsiteDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(fashionWebsiteDatabaseSettings.Value.DatabaseName);

        _productsCollection = mongoDatabase.GetCollection<Product>(fashionWebsiteDatabaseSettings.Value.ProductsCollectionName);

    }

    public async Task<List<Product>> Search(int limit) =>
        await _productsCollection.Find(_ => true).Limit(limit).ToListAsync();

    public async Task<Product?> Get(string id) =>
        await _productsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task Create(Product newProduct) =>
        await _productsCollection.InsertOneAsync(newProduct);

    public async Task Update(string id, Product updatedProduct) =>
        await _productsCollection.ReplaceOneAsync(x => x.Id == id, updatedProduct);

    public async Task Remove(string id) =>
        await _productsCollection.DeleteOneAsync(x => x.Id == id);
}