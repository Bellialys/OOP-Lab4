namespace OOP_Lab4.Models;

public class Customer
{
    public int CardNumber { get; }
    public decimal InitialMoney { get; }
    public decimal Money { get; private set; }
    public int PurchasedItems { get; private set; }
    public decimal SpentMoney { get; private set; }
    public CustomerType Type { get; }
    public decimal ShoppingBudget { get; }
    public int ShoppingItemLimit { get; }
    public List<Purchase> Cart { get; }

    public string TypeName => Type switch
    {
        CustomerType.Economical => "Економний",
        CustomerType.Spender => "Марнотратний",
        _ => "Звичайний"
    };

    public bool IsOverPlannedBudget => SpentMoney >= ShoppingBudget;

    public Customer(
        int cardNumber,
        decimal money,
        CustomerType type,
        decimal shoppingBudget,
        int shoppingItemLimit)
    {
        CardNumber = cardNumber;
        InitialMoney = money;
        Money = money;
        Type = type;
        ShoppingBudget = Math.Min(money, Math.Max(0, shoppingBudget));
        ShoppingItemLimit = Math.Clamp(shoppingItemLimit, 1, 15);
        PurchasedItems = 0;
        SpentMoney = 0;
        Cart = new List<Purchase>();
    }

    public bool CanBuy(decimal price, int quantity = 1)
    {
        if (price <= 0 || quantity <= 0)
            return false;

        return Money >= price * quantity;
    }

    public bool Buy(Product product, int quantity)
    {
        if (!CanBuy(product.Price, quantity))
            return false;

        decimal total = product.Price * quantity;

        Money -= total;
        SpentMoney += total;
        PurchasedItems += quantity;

        AddToCart(product, quantity);

        return true;
    }

    private void AddToCart(Product product, int quantity)
    {
        for (int i = 0; i < Cart.Count; i++)
        {
            if (Cart[i].ProductName == product.Name)
            {
                Purchase existing = Cart[i];

                Cart[i] = new Purchase(
                    existing.ProductName,
                    existing.Category,
                    existing.UnitPrice,
                    existing.Quantity + quantity);

                return;
            }
        }

        Cart.Add(
            new Purchase(
                product.Name,
                product.Category,
                product.Price,
                quantity));
    }
}
