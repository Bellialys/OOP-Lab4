namespace OOP_Lab4.Models;

public struct Purchase
{
    public string ProductName { get; set; }
    public string Category { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public decimal TotalPrice => UnitPrice * Quantity;

    public Purchase(
        string productName,
        string category,
        decimal unitPrice,
        int quantity)
    {
        ProductName = productName;
        Category = category;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}
