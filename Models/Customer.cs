namespace OOP_Lab4.Models;

public class Customer
{
    public int CardNumber { get; }
    public decimal InitialMoney { get; }
    public decimal Money { get; private set; }
    public int PurchasedItems { get; private set; }
    public decimal SpentMoney { get; private set; }

    public Customer(int cardNumber, decimal money)
    {
        CardNumber = cardNumber;
        InitialMoney = money;
        Money = money;
        PurchasedItems = 0;
        SpentMoney = 0;
    }

    public bool CanBuy(decimal price)
    {
        return price > 0 && Money >= price;
    }

    public bool Buy(decimal price)
    {
        if (!CanBuy(price))
            return false;

        Money -= price;
        SpentMoney += price;
        PurchasedItems++;
        return true;
    }
}
