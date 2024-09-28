using dotnet.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace dotnet.Services;

public class CartsService
{
    readonly IMongoCollection<Cart> _cartsCollection;

    public CartsService(IOptions<FashionWebsiteDatabaseSettings> fashionWebsiteDatabaseSettings)
    {
        var mongoClient = new MongoClient(fashionWebsiteDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(fashionWebsiteDatabaseSettings.Value.DatabaseName);

        _cartsCollection = mongoDatabase.GetCollection<Cart>(fashionWebsiteDatabaseSettings.Value.CartsCollectionName);
    }

    public async Task<List<Cart>> GetAsync(int limit) =>
        await _cartsCollection.Find(_ => true).Limit(limit).ToListAsync();

    public async Task<Cart?> GetAsync(string id) =>
        await _cartsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Cart newCart) =>
        await _cartsCollection.InsertOneAsync(newCart);

    public async Task UpdateAsync(string id, Cart updatedCart) =>
        await _cartsCollection.ReplaceOneAsync(x => x.Id == id, updatedCart);

    public async Task RemoveAsync(string id) =>
        await _cartsCollection.DeleteOneAsync(x => x.Id == id);
}
