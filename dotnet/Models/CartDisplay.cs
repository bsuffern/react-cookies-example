namespace dotnet.Models;

public class CartDisplay
{
    public int Quantity { get; set; }
    public Product Product { get; set; } = null!;
}
