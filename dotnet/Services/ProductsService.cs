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

    public async Task<List<Product>> GetAsync() =>
        await _productsCollection.Find(_ => true).ToListAsync();

    public async Task<Product?> GetAsync(string id) =>
        await _productsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Product newProduct) =>
        await _productsCollection.InsertOneAsync(newProduct);

    public async Task UpdateAsync(string id, Product updatedProduct) =>
        await _productsCollection.ReplaceOneAsync(x => x.Id == id, updatedProduct);

    public async Task RemoveAsync(string id) =>
        await _productsCollection.DeleteOneAsync(x => x.Id == id);
}