namespace dotnet.Models
{
    public class FashionWebsiteDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string ProductsCollectionName { get; set; } = null!;
        public string CartsCollectionName { get; set; } = null!;
    }
}
