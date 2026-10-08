using OOP_Lab4.Models;

namespace OOP_Lab4.Services;

public class Store
{
    public Product[] Products { get; }
    public decimal Profit { get; private set; }
    public int TotalItemsSold { get; private set; }
    public int PayingCustomers { get; private set; }
    public int ProcessedCustomers { get; private set; }

    public Store()
    {
        Profit = 0;
        TotalItemsSold = 0;
        PayingCustomers = 0;
        ProcessedCustomers = 0;

        Products = new Product[]
        {
            new Product("Хліб", "Випічка", 45, 30),
            new Product("Молоко", "Молочні", 68, 25),
            new Product("Яблука", "Фрукти", 79, 25),
            new Product("Шоколад", "Солодощі", 89, 20),
            new Product("Рис", "Крупи", 105, 20),
            new Product("Чай", "Напої", 135, 18),
            new Product("Курятина", "М'ясо", 189, 16),
            new Product("Сир", "Молочні", 225, 15),
            new Product("Кава", "Напої", 310, 12),
            new Product("Оливкова олія", "Бакалія", 420, 10)
        };
    }

    public Customer[] CreateCustomers(int count, Random random)
    {
        if (count < 1 || count > 1000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                "Кількість покупців повинна бути від 1 до 1000.");
        }

        Customer[] customers = new Customer[count];
        bool[] usedCardNumbers = new bool[1001];

        for (int i = 0; i < customers.Length; i++)
        {
            int cardNumber;

            do
            {
                cardNumber = random.Next(1, 1001);
            }
            while (usedCardNumbers[cardNumber]);

            usedCardNumbers[cardNumber] = true;

            decimal money = random.Next(1000, 10001);
            CustomerType type = (CustomerType)random.Next(0, 3);

            int budgetPercent = type switch
            {
                CustomerType.Economical => random.Next(35, 56),
                CustomerType.Spender => random.Next(75, 101),
                _ => random.Next(55, 81)
            };

            decimal shoppingBudget = Math.Round(
                money * budgetPercent / 100m,
                2);

            customers[i] = new Customer(
                cardNumber,
                money,
                type,
                shoppingBudget);
        }

        return customers;
    }

    public void RunSimulation(Customer[] customers, Random random)
    {
        Console.WriteLine();
        Console.WriteLine("========== СИМУЛЯЦІЯ ПОКУПОК ==========");

        for (int i = 0; i < customers.Length; i++)
        {
            if (AreAllProductsOutOfStock())
            {
                int remainingCustomers = customers.Length - i;

                Console.WriteLine();
                Console.WriteLine(
                    "У магазині закінчилися всі товари.");
                Console.WriteLine(
                    $"Без покупок залишилося покупців: {remainingCustomers}.");

                break;
            }

            Customer customer = customers[i];
            ProcessedCustomers++;

            Console.WriteLine();
            Console.WriteLine($"Покупець  : {i + 1}");
            Console.WriteLine($"Картка    : №{customer.CardNumber}");
            Console.WriteLine($"Тип       : {customer.TypeName}");
            Console.WriteLine($"Кошти     : {customer.InitialMoney:N2} грн");
            Console.WriteLine($"План      : {customer.ShoppingBudget:N2} грн");

            int lastProductIndex = -1;

            while (true)
            {
                int productIndex = FindPreferredAffordableProductIndex(
                    customer,
                    random,
                    lastProductIndex);

                if (productIndex == -1)
                    break;

                Product selectedProduct = Products[productIndex];

                int quantity = GetPurchaseQuantity(
                    customer,
                    selectedProduct,
                    random);

                if (quantity <= 0)
                    break;

                if (!TrySellProduct(
                        customer,
                        productIndex,
                        quantity,
                        out Purchase purchase))
                {
                    break;
                }

                Console.WriteLine(
                    $"  У кошик: {purchase.ProductName,-18} " +
                    $"{purchase.Quantity} шт. x " +
                    $"{purchase.UnitPrice:N2} грн = " +
                    $"{purchase.TotalPrice:N2} грн");

                lastProductIndex = productIndex;
            }

            if (customer.SpentMoney > 0)
                PayingCustomers++;

            ShowReceipt(customer, i + 1);
        }
    }

    public bool TrySellProduct(
        Customer customer,
        int productIndex,
        int quantity,
        out Purchase purchase)
    {
        purchase = default;

        if (productIndex < 0 || productIndex >= Products.Length)
            return false;

        if (quantity <= 0)
            return false;

        Product product = Products[productIndex];

        if (product.Quantity < quantity)
            return false;

        if (!customer.CanBuy(product.Price, quantity))
            return false;

        if (!customer.Buy(product, quantity))
            return false;

        product.Quantity -= quantity;
        Products[productIndex] = product;

        decimal total = product.Price * quantity;

        Profit += total;
        TotalItemsSold += quantity;

        purchase = new Purchase(
            product.Name,
            product.Category,
            product.Price,
            quantity);

        return true;
    }

    public void ShowProducts()
    {
        Console.WriteLine();
        Console.WriteLine(
            "=================== ТОВАРИ В МАГАЗИНІ ===================");
        Console.WriteLine(
            "Товар              | Категорія      |     Ціна | Залишок");
        Console.WriteLine(
            "-----------------------------------------------------------");

        foreach (Product product in Products)
        {
            Console.WriteLine(
                $"{product.Name,-18} | " +
                $"{product.Category,-14} | " +
                $"{product.Price,8:N2} | " +
                $"{product.Quantity,7}");
        }
    }

    public void ShowInventoryChanges()
    {
        Console.WriteLine();
        Console.WriteLine(
            "================ РУХ ТОВАРІВ І ЗАЛИШКИ =================");
        Console.WriteLine(
            "Товар              | Було | Продано | Залишилось");
        Console.WriteLine(
            "----------------------------------------------------------");

        foreach (Product product in Products)
        {
            Console.WriteLine(
                $"{product.Name,-18} | " +
                $"{product.InitialQuantity,4} | " +
                $"{product.SoldQuantity,7} | " +
                $"{product.Quantity,10}");
        }
    }

    public void ShowResults(Customer[] customers)
    {
        Console.WriteLine();
        Console.WriteLine(
            "================ СТАТИСТИКА МАГАЗИНУ =================");

        decimal averageReceipt =
            PayingCustomers > 0
                ? Profit / PayingCustomers
                : 0;

        Console.WriteLine($"Заплановано покупців: {customers.Length}");
        Console.WriteLine($"Обслуговано покупців: {ProcessedCustomers}");
        Console.WriteLine($"Покупців із покупками: {PayingCustomers}");
        Console.WriteLine(
            $"Не обслуговано через відсутність товару: " +
            $"{customers.Length - ProcessedCustomers}");
        Console.WriteLine($"Продано одиниць товару: {TotalItemsSold}");
        Console.WriteLine($"Прибуток магазину: {Profit:N2} грн");
        Console.WriteLine($"Середній чек: {averageReceipt:N2} грн");

        if (customers.Length == 0)
            return;

        Customer maxItemsCustomer = customers[0];
        Customer maxSpentCustomer = customers[0];

        foreach (Customer customer in customers)
        {
            if (customer.PurchasedItems >
                maxItemsCustomer.PurchasedItems)
            {
                maxItemsCustomer = customer;
            }

            if (customer.SpentMoney >
                maxSpentCustomer.SpentMoney)
            {
                maxSpentCustomer = customer;
            }
        }

        Product mostPopular = Products[0];
        Product leastPopular = Products[0];

        foreach (Product product in Products)
        {
            if (product.SoldQuantity > mostPopular.SoldQuantity)
                mostPopular = product;

            if (product.SoldQuantity < leastPopular.SoldQuantity)
                leastPopular = product;
        }

        Console.WriteLine();
        Console.WriteLine(
            "Покупець, який придбав найбільшу кількість товарів:");
        Console.WriteLine(
            $"Картка №{maxItemsCustomer.CardNumber}, " +
            $"товарів: {maxItemsCustomer.PurchasedItems}");

        Console.WriteLine();
        Console.WriteLine(
            "Покупець, який витратив найбільше грошей:");
        Console.WriteLine(
            $"Картка №{maxSpentCustomer.CardNumber}, " +
            $"витрачено: {maxSpentCustomer.SpentMoney:N2} грн");

        Console.WriteLine();
        Console.WriteLine(
            $"Найпопулярніший товар: {mostPopular.Name} — " +
            $"{mostPopular.SoldQuantity} шт.");
        Console.WriteLine(
            $"Найменш популярний товар: {leastPopular.Name} — " +
            $"{leastPopular.SoldQuantity} шт.");

        Console.WriteLine();
        Console.WriteLine("Статистика всіх покупців:");

        for (int i = 0; i < customers.Length; i++)
        {
            Customer customer = customers[i];

            Console.WriteLine(
                $"{i + 1,3}. Картка №{customer.CardNumber,-4} | " +
                $"{customer.TypeName,-11} | " +
                $"Старт: {customer.InitialMoney,9:N2} | " +
                $"План: {customer.ShoppingBudget,9:N2} | " +
                $"Залишок: {customer.Money,9:N2} | " +
                $"Товарів: {customer.PurchasedItems,3} | " +
                $"Витрачено: {customer.SpentMoney,9:N2}");
        }
    }

    private int FindPreferredAffordableProductIndex(
        Customer customer,
        Random random,
        int lastProductIndex)
    {
        List<int> availableIndexes = new List<int>();

        for (int i = 0; i < Products.Length; i++)
        {
            if (Products[i].Quantity > 0 &&
                customer.CanBuy(Products[i].Price))
            {
                availableIndexes.Add(i);
            }
        }

        if (availableIndexes.Count == 0)
            return -1;

        int selectedIndex;

        bool chooseEconomically =
            customer.Type == CustomerType.Economical ||
            customer.IsOverPlannedBudget;

        if (chooseEconomically && random.Next(100) < 75)
        {
            selectedIndex = FindCheapestProductIndex(
                availableIndexes);
        }
        else if (
            customer.Type == CustomerType.Spender &&
            random.Next(100) < 75)
        {
            selectedIndex = FindMostExpensiveProductIndex(
                availableIndexes);
        }
        else
        {
            selectedIndex =
                availableIndexes[random.Next(availableIndexes.Count)];
        }

        if (availableIndexes.Count > 1 &&
            selectedIndex == lastProductIndex)
        {
            int currentPosition =
                availableIndexes.IndexOf(selectedIndex);

            selectedIndex =
                availableIndexes[
                    (currentPosition + 1 +
                     random.Next(availableIndexes.Count - 1)) %
                    availableIndexes.Count];
        }

        return selectedIndex;
    }

    private int GetPurchaseQuantity(
        Customer customer,
        Product product,
        Random random)
    {
        int maxByMoney = (int)(customer.Money / product.Price);

        int maxQuantity = Math.Min(
            3,
            Math.Min(product.Quantity, maxByMoney));

        if (maxQuantity <= 0)
            return 0;

        if (customer.IsOverPlannedBudget)
            return 1;

        if (customer.Type == CustomerType.Economical)
        {
            int economicalMax = Math.Min(2, maxQuantity);
            return random.Next(1, economicalMax + 1);
        }

        if (customer.Type == CustomerType.Spender &&
            maxQuantity >= 2)
        {
            return random.Next(2, maxQuantity + 1);
        }

        return random.Next(1, maxQuantity + 1);
    }

    private int FindCheapestProductIndex(List<int> indexes)
    {
        int result = indexes[0];

        foreach (int index in indexes)
        {
            if (Products[index].Price < Products[result].Price)
                result = index;
        }

        return result;
    }

    private int FindMostExpensiveProductIndex(List<int> indexes)
    {
        int result = indexes[0];

        foreach (int index in indexes)
        {
            if (Products[index].Price > Products[result].Price)
                result = index;
        }

        return result;
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

    private void ShowReceipt(Customer customer, int receiptNumber)
    {
        Console.WriteLine();
        Console.WriteLine("==============================================");
        Console.WriteLine($"               ЧЕК №{receiptNumber:0000}");
        Console.WriteLine("==============================================");
        Console.WriteLine($"Картка покупця: №{customer.CardNumber}");
        Console.WriteLine($"Тип покупця: {customer.TypeName}");
        Console.WriteLine(
            $"Початкові кошти: {customer.InitialMoney:N2} грн");
        Console.WriteLine(
            $"Плановий бюджет: {customer.ShoppingBudget:N2} грн");
        Console.WriteLine("----------------------------------------------");

        if (customer.Cart.Count == 0)
        {
            Console.WriteLine("Покупок немає.");
        }
        else
        {
            foreach (Purchase purchase in customer.Cart)
            {
                Console.WriteLine(
                    $"{purchase.ProductName,-18} " +
                    $"{purchase.Quantity,2} x " +
                    $"{purchase.UnitPrice,7:N2} = " +
                    $"{purchase.TotalPrice,9:N2} грн");
            }
        }

        Console.WriteLine("----------------------------------------------");
        Console.WriteLine(
            $"Кількість товарів: {customer.PurchasedItems}");
        Console.WriteLine(
            $"До сплати: {customer.SpentMoney:N2} грн");
        Console.WriteLine(
            $"Залишок коштів: {customer.Money:N2} грн");

        if (customer.SpentMoney > customer.ShoppingBudget)
        {
            Console.WriteLine(
                $"Перевищення планового бюджету: " +
                $"{customer.SpentMoney - customer.ShoppingBudget:N2} грн");
        }

        Console.WriteLine("==============================================");
    }
}
