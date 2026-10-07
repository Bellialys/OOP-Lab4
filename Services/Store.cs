using OOP_Lab4.Models;

namespace OOP_Lab4.Services;

public class Store
{
    public Product[] Products { get; }
    public decimal Profit { get; private set; }

    public Store()
    {
        Profit = 0;

        Products = new Product[]
        {
            new Product("Хліб", 45, 30),
            new Product("Молоко", 68, 25),
            new Product("Яблука", 79, 25),
            new Product("Шоколад", 89, 20),
            new Product("Рис", 105, 20),
            new Product("Чай", 135, 18),
            new Product("Курятина", 189, 16),
            new Product("Сир", 225, 15),
            new Product("Кава", 310, 12),
            new Product("Оливкова олія", 420, 10)
        };
    }

    public Customer[] CreateCustomers(int count, Random random)
    {
        Customer[] customers = new Customer[count];

        for (int i = 0; i < customers.Length; i++)
        {
            int cardNumber = random.Next(1, 1001);
            decimal money = random.Next(1000, 10001);
            customers[i] = new Customer(cardNumber, money);
        }

        return customers;
    }

    public void RunSimulation(Customer[] customers, Random random)
    {
        Console.WriteLine();
        Console.WriteLine("========== СИМУЛЯЦІЯ ПОКУПОК ==========");

        for (int i = 0; i < customers.Length; i++)
        {
            Customer customer = customers[i];

            Console.WriteLine();
            Console.WriteLine($"Покупець {i + 1}: картка №{customer.CardNumber}, початкові кошти {customer.InitialMoney:N2} грн");

            while (true)
            {
                int productIndex = FindRandomAffordableProductIndex(customer, random);

                if (productIndex == -1)
                    break;

                if (TrySellProduct(customer, productIndex, out Product soldProduct))
                {
                    Console.WriteLine($"  Куплено: {soldProduct.Name,-18} за {soldProduct.Price,8:N2} грн | Залишок: {customer.Money,8:N2} грн");
                }
            }

            Console.WriteLine($"  Підсумок: товарів {customer.PurchasedItems}, витрачено {customer.SpentMoney:N2} грн, залишок {customer.Money:N2} грн.");

            if (AreAllProductsOutOfStock())
            {
                Console.WriteLine();
                Console.WriteLine("У магазині закінчилися всі товари.");
                break;
            }
        }
    }

    public bool TrySellProduct(Customer customer, int productIndex, out Product soldProduct)
    {
        soldProduct = default;

        if (productIndex < 0 || productIndex >= Products.Length)
            return false;

        Product product = Products[productIndex];

        if (product.Quantity <= 0 || !customer.CanBuy(product.Price))
            return false;

        if (!customer.Buy(product.Price))
            return false;

        product.Quantity--;
        Products[productIndex] = product;
        Profit += product.Price;
        soldProduct = product;

        return true;
    }

    public void ShowProducts()
    {
        Console.WriteLine();
        Console.WriteLine("========== ТОВАРИ В МАГАЗИНІ ==========");

        foreach (Product product in Products)
        {
            Console.WriteLine(product);
        }
    }

    public void ShowResults(Customer[] customers)
    {
        Console.WriteLine();
        Console.WriteLine("========== РЕЗУЛЬТАТИ МАГАЗИНУ ==========");
        Console.WriteLine($"Прибуток магазину: {Profit:N2} грн");

        if (customers.Length == 0)
            return;

        Customer maxItemsCustomer = customers[0];
        Customer maxSpentCustomer = customers[0];

        foreach (Customer customer in customers)
        {
            if (customer.PurchasedItems > maxItemsCustomer.PurchasedItems)
                maxItemsCustomer = customer;

            if (customer.SpentMoney > maxSpentCustomer.SpentMoney)
                maxSpentCustomer = customer;
        }

        Console.WriteLine();
        Console.WriteLine("Покупець, який придбав найбільшу кількість товарів:");
        Console.WriteLine($"Картка №{maxItemsCustomer.CardNumber}, товарів: {maxItemsCustomer.PurchasedItems}");

        Console.WriteLine();
        Console.WriteLine("Покупець, який витратив найбільше грошей:");
        Console.WriteLine($"Картка №{maxSpentCustomer.CardNumber}, витрачено: {maxSpentCustomer.SpentMoney:N2} грн");

        Console.WriteLine();
        Console.WriteLine("Статистика всіх покупців:");

        for (int i = 0; i < customers.Length; i++)
        {
            Customer customer = customers[i];

            Console.WriteLine($"{i + 1,3}. Картка №{customer.CardNumber,-4} | Старт: {customer.InitialMoney,9:N2} | Залишок: {customer.Money,9:N2} | Товарів: {customer.PurchasedItems,3} | Витрачено: {customer.SpentMoney,9:N2}");
        }
    }

    private int FindRandomAffordableProductIndex(Customer customer, Random random)
    {
        List<int> availableIndexes = new List<int>();

        for (int i = 0; i < Products.Length; i++)
        {
            if (Products[i].Quantity > 0 && customer.CanBuy(Products[i].Price))
                availableIndexes.Add(i);
        }

        if (availableIndexes.Count == 0)
            return -1;

        return availableIndexes[random.Next(availableIndexes.Count)];
    }

    private bool AreAllProductsOutOfStock()
    {
        foreach (Product product in Products)
        {
            if (product.Quantity > 0)
                return false;
        }

        return true;
    }
}
