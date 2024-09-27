namespace dotnet.Models;

public class PutCartRequest
{
    public string CartId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public bool IncreaseQuantity { get; set; }
}
