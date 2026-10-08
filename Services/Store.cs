using OOP_Lab4.Models;

namespace OOP_Lab4.Services;

public class Store
{
    private const int MaxStockPerProduct = 250;

    public Product[] Products { get; }
    public decimal Profit { get; private set; }
    public int TotalItemsSold { get; private set; }
    public int PayingCustomers { get; private set; }
    public int ProcessedCustomers { get; private set; }

    public Store()
        : this(Random.Shared)
    {
    }

    public Store(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);

        Profit = 0;
        TotalItemsSold = 0;
        PayingCustomers = 0;
        ProcessedCustomers = 0;

        Products = new Product[]
        {
            CreateProduct("Хліб", "Випічка", 45, random),
            CreateProduct("Батон", "Випічка", 38, random),
            CreateProduct("Круасан", "Випічка", 52, random),

            CreateProduct("Молоко", "Молочні", 68, random),
            CreateProduct("Кефір", "Молочні", 72, random),
            CreateProduct("Йогурт", "Молочні", 55, random),
            CreateProduct("Сир", "Молочні", 225, random),
            CreateProduct("Масло", "Молочні", 98, random),

            CreateProduct("Яблука", "Фрукти", 79, random),
            CreateProduct("Банани", "Фрукти", 95, random),
            CreateProduct("Апельсини", "Фрукти", 110, random),

            CreateProduct("Шоколад", "Солодощі", 89, random),
            CreateProduct("Печиво", "Солодощі", 65, random),

            CreateProduct("Рис", "Крупи", 105, random),
            CreateProduct("Гречка", "Крупи", 115, random),
            CreateProduct("Макарони", "Бакалія", 75, random),
            CreateProduct("Оливкова олія", "Бакалія", 420, random),

            CreateProduct("Чай", "Напої", 135, random),
            CreateProduct("Кава", "Напої", 310, random),
            CreateProduct("Вода", "Напої", 32, random),
            CreateProduct("Сік", "Напої", 78, random),

            CreateProduct("Курятина", "М'ясо", 189, random),
            CreateProduct("Сосиски", "М'ясо", 170, random),

            CreateProduct("Яйця", "Продукти", 92, random)
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

            int shoppingItemLimit = random.Next(1, 16);

            customers[i] = new Customer(
                cardNumber,
                money,
                type,
                shoppingBudget,
                shoppingItemLimit);
        }

        return customers;
    }

    public void RunSimulation(
        Customer[] customers,
        Random random,
        bool realisticMode)
    {
        ConsoleUI.WriteSection("СИМУЛЯЦІЯ ПОКУПОК");

        ConsoleUI.WriteInfo(
            realisticMode
                ? "Режим: реалістичний, кожен покупець має випадковий ліміт 1–15 товарів."
                : "Режим: за умовою завдання, покупки тривають до вичерпання грошей або товарів.");

        for (int i = 0; i < customers.Length; i++)
        {
            if (AreAllProductsOutOfStock())
            {
                int remainingCustomers = customers.Length - i;

                Console.WriteLine();
                ConsoleUI.WriteWarning(
                    "У магазині закінчилися всі товари.");
                ConsoleUI.WriteWarning(
                    $"Без покупок залишилося покупців: {remainingCustomers}.");

                break;
            }

            Customer customer = customers[i];
            ProcessedCustomers++;

            ConsoleUI.WriteSection(
                $"ПОКУПЕЦЬ {i + 1} / {customers.Length}");

            ConsoleUI.WriteLabel(
                "Картка:",
                $"№{customer.CardNumber}");
            ConsoleUI.WriteLabel(
                "Тип покупця:",
                customer.TypeName);
            ConsoleUI.WriteLabel(
                "Початкові кошти:",
                $"{customer.InitialMoney:N2} грн");
            ConsoleUI.WriteLabel(
                "Плановий бюджет:",
                $"{customer.ShoppingBudget:N2} грн");

            if (realisticMode)
            {
                ConsoleUI.WriteLabel(
                    "Ліміт кошика:",
                    $"{customer.ShoppingItemLimit} од.");
            }

            int lastProductIndex = -1;

            while (true)
            {
                if (realisticMode &&
                    customer.PurchasedItems >= customer.ShoppingItemLimit)
                {
                    ConsoleUI.WriteInfo(
                        $"Досягнуто ліміт кошика: {customer.ShoppingItemLimit} од.");
                    break;
                }

                int productIndex = FindPreferredAffordableProductIndex(
                    customer,
                    random,
                    lastProductIndex);

                if (productIndex == -1)
                    break;

                Product selectedProduct = Products[productIndex];

                int remainingLimit = realisticMode
                    ? customer.ShoppingItemLimit - customer.PurchasedItems
                    : int.MaxValue;

                int quantity = GetPurchaseQuantity(
                    customer,
                    selectedProduct,
                    random,
                    remainingLimit);

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
                    $"  + {purchase.ProductName,-18} " +
                    $"{purchase.Quantity} шт. x " +
                    $"{purchase.UnitPrice:N2} грн = " +
                    $"{purchase.TotalPrice:N2} грн");

                lastProductIndex = productIndex;
            }

            if (customer.SpentMoney > 0)
                PayingCustomers++;

            ShowReceipt(
                customer,
                i + 1,
                realisticMode);
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
        ConsoleUI.WriteSection("АСОРТИМЕНТ МАГАЗИНУ");

        Console.WriteLine(
            " # | Товар              | Категорія      |     Ціна | Склад");
        Console.WriteLine(
            "---+--------------------+----------------+----------+------------------");

        for (int i = 0; i < Products.Length; i++)
        {
            Product product = Products[i];

            string stockBar = ConsoleUI.BuildBar(
                product.Quantity,
                MaxStockPerProduct);

            Console.WriteLine(
                $"{i + 1,2} | " +
                $"{product.Name,-18} | " +
                $"{product.Category,-14} | " +
                $"{product.Price,8:N2} | " +
                $"{stockBar} {product.Quantity,3}");
        }

        Console.WriteLine();
        ConsoleUI.WriteInfo(
            $"Позицій в асортименті: {Products.Length}. " +
            $"Кількість кожного товару генерується випадково від 40 до {MaxStockPerProduct}.");
    }

    public void ShowInventoryChanges()
    {
        ConsoleUI.WriteSection("РУХ ТОВАРІВ І ЗАЛИШКИ");

        Console.WriteLine(
            "Товар              | Було | Продано | Залишок | Стан");
        Console.WriteLine(
            "-------------------+------+---------+---------+--------------");

        foreach (Product product in Products)
        {
            string bar = ConsoleUI.BuildBar(
                product.Quantity,
                product.InitialQuantity,
                10);

            Console.WriteLine(
                $"{product.Name,-18} | " +
                $"{product.InitialQuantity,4} | " +
                $"{product.SoldQuantity,7} | " +
                $"{product.Quantity,7} | " +
                $"{bar}");
        }
    }

    public void ShowResults(Customer[] customers)
    {
        ConsoleUI.WriteSection("СТАТИСТИКА МАГАЗИНУ");

        decimal averageReceipt =
            PayingCustomers > 0
                ? Profit / PayingCustomers
                : 0;

        ConsoleUI.WriteLabel(
            "Заплановано:",
            $"{customers.Length} покупців");
        ConsoleUI.WriteLabel(
            "Обслуговано:",
            $"{ProcessedCustomers} покупців");
        ConsoleUI.WriteLabel(
            "З покупками:",
            $"{PayingCustomers} покупців");
        ConsoleUI.WriteLabel(
            "Не обслуговано:",
            $"{customers.Length - ProcessedCustomers}");
        ConsoleUI.WriteLabel(
            "Продано товарів:",
            $"{TotalItemsSold} од.");
        ConsoleUI.WriteLabel(
            "Прибуток:",
            $"{Profit:N2} грн");
        ConsoleUI.WriteLabel(
            "Середній чек:",
            $"{averageReceipt:N2} грн");

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
        ConsoleUI.WriteSuccess(
            $"Найбільше товарів: картка №{maxItemsCustomer.CardNumber} — " +
            $"{maxItemsCustomer.PurchasedItems} од.");
        ConsoleUI.WriteSuccess(
            $"Найбільші витрати: картка №{maxSpentCustomer.CardNumber} — " +
            $"{maxSpentCustomer.SpentMoney:N2} грн");
        ConsoleUI.WriteInfo(
            $"Найпопулярніший товар: {mostPopular.Name} — " +
            $"{mostPopular.SoldQuantity} од.");
        ConsoleUI.WriteInfo(
            $"Найменш популярний товар: {leastPopular.Name} — " +
            $"{leastPopular.SoldQuantity} од.");

        Console.WriteLine();
        Console.WriteLine(
            " # | Картка | Тип         | Товарів | Витрачено   | Залишок");
        Console.WriteLine(
            "---+--------+-------------+---------+-------------+-------------");

        for (int i = 0; i < customers.Length; i++)
        {
            Customer customer = customers[i];

            Console.WriteLine(
                $"{i + 1,2} | " +
                $"{customer.CardNumber,6} | " +
                $"{customer.TypeName,-11} | " +
                $"{customer.PurchasedItems,7} | " +
                $"{customer.SpentMoney,11:N2} | " +
                $"{customer.Money,11:N2}");
        }
    }

    private static Product CreateProduct(
        string name,
        string category,
        decimal price,
        Random random)
    {
        int quantity = random.Next(40, MaxStockPerProduct + 1);

        return new Product(
            name,
            category,
            price,
            quantity);
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
        Random random,
        int remainingLimit)
    {
        int maxByMoney = (int)(customer.Money / product.Price);

        int maxQuantity = Math.Min(
            3,
            Math.Min(
                product.Quantity,
                Math.Min(maxByMoney, remainingLimit)));

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

    private void ShowReceipt(
        Customer customer,
        int receiptNumber,
        bool realisticMode)
    {
        ConsoleUI.WriteSection(
            $"ЧЕК №{receiptNumber:0000}");

        ConsoleUI.WriteLabel(
            "Картка покупця:",
            $"№{customer.CardNumber}");
        ConsoleUI.WriteLabel(
            "Тип покупця:",
            customer.TypeName);
        ConsoleUI.WriteLabel(
            "Початкові кошти:",
            $"{customer.InitialMoney:N2} грн");
        ConsoleUI.WriteLabel(
            "Плановий бюджет:",
            $"{customer.ShoppingBudget:N2} грн");

        if (realisticMode)
        {
            ConsoleUI.WriteLabel(
                "Ліміт кошика:",
                $"{customer.ShoppingItemLimit} од.");
        }

        Console.WriteLine(
            "----------------------------------------------------------------");
        Console.WriteLine(
            "Товар              | К-сть | Ціна       | Сума");
        Console.WriteLine(
            "-------------------+-------+------------+-------------");

        if (customer.Cart.Count == 0)
        {
            ConsoleUI.WriteWarning("Покупок немає.");
        }
        else
        {
            foreach (Purchase purchase in customer.Cart)
            {
                Console.WriteLine(
                    $"{purchase.ProductName,-18} | " +
                    $"{purchase.Quantity,5} | " +
                    $"{purchase.UnitPrice,10:N2} | " +
                    $"{purchase.TotalPrice,11:N2}");
            }
        }

        Console.WriteLine(
            "----------------------------------------------------------------");

        ConsoleUI.WriteLabel(
            "Кількість товарів:",
            $"{customer.PurchasedItems} од.");
        ConsoleUI.WriteLabel(
            "До сплати:",
            $"{customer.SpentMoney:N2} грн");
        ConsoleUI.WriteLabel(
            "Залишок коштів:",
            $"{customer.Money:N2} грн");

        if (customer.SpentMoney > customer.ShoppingBudget)
        {
            ConsoleUI.WriteWarning(
                $"Плановий бюджет перевищено на " +
                $"{customer.SpentMoney - customer.ShoppingBudget:N2} грн.");
        }
        else
        {
            ConsoleUI.WriteSuccess(
                "Покупець залишився в межах планового бюджету.");
        }
    }
}
