namespace OOP_Lab4.Models;

public struct Product
{
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int InitialQuantity { get; }

    public int SoldQuantity => InitialQuantity - Quantity;

    public Product(
        string name,
        string category,
        decimal price,
        int quantity)
    {
        Name = name;
        Category = category;
        Price = price;
        Quantity = quantity;
        InitialQuantity = quantity;
    }

    public override string ToString()
    {
        return $"{Name,-18} | {Category,-14} | {Price,8:N2} грн | Кількість: {Quantity}";
    }
}
