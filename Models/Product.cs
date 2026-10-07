namespace OOP_Lab4.Models;

public struct Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public Product(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    public override string ToString()
    {
        return $"{Name,-18} | {Price,8:N2} грн | Кількість: {Quantity}";
    }
}
