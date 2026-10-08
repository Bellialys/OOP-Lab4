using System.Text;
using OOP_Lab4.Models;
using OOP_Lab4.Services;
using OOP_Lab4.Tests;

namespace OOP_Lab4;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        if (args.Length > 0 && args[0] == "--self-test")
        {
            Environment.ExitCode = SelfTests.Run();
            return;
        }

        try
        {
            RunApplication();
        }
        catch (EndOfStreamException)
        {
            ConsoleUI.WriteWarning(
                "Ввід завершено. Програму коректно закрито.");
        }
    }

    private static void RunApplication()
    {
        SocialNetwork socialNetwork = new SocialNetwork();

        while (true)
        {
            PrintHeader();

            ConsoleUI.WriteMenuItem(
                1,
                "Симуляція покупців у магазині");
            ConsoleUI.WriteMenuItem(
                2,
                "Симуляція поведінки користувачів у соціальній мережі");
            ConsoleUI.WriteMenuExit("Вихід");
            Console.WriteLine();

            int choice = ConsoleHelper.ReadIntInRange(
                "Ваш вибір: ",
                0,
                2);

            switch (choice)
            {
                case 1:
                    RunStoreMenu();
                    break;

                case 2:
                    socialNetwork.Run();
                    break;

                case 0:
                    ConsoleUI.WriteSuccess(
                        "Роботу програми завершено.");
                    return;
            }
        }
    }

    private static void PrintHeader()
    {
        ConsoleUI.WriteBanner(
            "ФІТ-2-15",
            "ПРАКТИЧНЕ ЗАВДАННЯ №4 З ООП",
            "СТРУКТУРИ, КЛАСИ ТА ФУНКЦІЇ");
    }

    private static void RunStoreMenu()
    {
        Random random = new Random();
        Store store = new Store(random);
        Customer[]? customers = null;

        while (true)
        {
            ConsoleUI.WriteBanner(
                "МАГАЗИН",
                "СИМУЛЯЦІЯ ПОКУПОК");

            ConsoleUI.WriteMenuItem(1, "Переглянути асортимент");
            ConsoleUI.WriteMenuItem(2, "Запустити нову симуляцію покупців");
            ConsoleUI.WriteMenuItem(3, "Переглянути рух товарів і залишки");
            ConsoleUI.WriteMenuItem(4, "Переглянути статистику магазину");
            ConsoleUI.WriteMenuExit("Повернутися до головного меню");
            Console.WriteLine();

            int choice = ConsoleHelper.ReadIntInRange(
                "Ваш вибір: ",
                0,
                4);

            switch (choice)
            {
                case 1:
                {
                    store.ShowProducts();
                    ConsoleHelper.Pause();
                    break;
                }

                case 2:
                {
                    ConsoleUI.WriteSection("РЕЖИМ СИМУЛЯЦІЇ");
                    ConsoleUI.WriteMenuItem(
                        1,
                        "Реалістичний: випадковий ліміт 1–15 товарів на покупця");
                    ConsoleUI.WriteMenuItem(
                        2,
                        "За умовою завдання: покупки до вичерпання грошей або товарів");

                    int mode = ConsoleHelper.ReadIntInRange(
                        "Оберіть режим: ",
                        1,
                        2);

                    bool realisticMode = mode == 1;

                    int customerCount = ConsoleHelper.ReadIntInRange(
                        "Введіть кількість покупців (від 1 до 1000): ",
                        1,
                        1000);

                    store = new Store(random);
                    customers = store.CreateCustomers(
                        customerCount,
                        random);

                    store.ShowProducts();
                    store.RunSimulation(
                        customers,
                        random,
                        realisticMode);
                    store.ShowInventoryChanges();
                    store.ShowResults(customers);

                    ConsoleHelper.Pause();
                    break;
                }

                case 3:
                {
                    if (customers == null)
                    {
                        ConsoleUI.WriteWarning(
                            "Спочатку запустіть симуляцію покупців.");
                    }
                    else
                    {
                        store.ShowInventoryChanges();
                    }

                    ConsoleHelper.Pause();
                    break;
                }

                case 4:
                {
                    if (customers == null)
                    {
                        ConsoleUI.WriteWarning(
                            "Спочатку запустіть симуляцію покупців.");
                    }
                    else
                    {
                        store.ShowResults(customers);
                    }

                    ConsoleHelper.Pause();
                    break;
                }

                case 0:
                    return;
            }
        }
    }
}
