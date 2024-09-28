namespace dotnet.Models;

public class GetCartResponse
{
    public int Quantity { get; set; }
    public Product Product { get; set; } = null!;
}
