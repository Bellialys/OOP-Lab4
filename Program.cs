using System.Text;
using OOP_Lab4.Models;
using OOP_Lab4.Services;

namespace OOP_Lab4;

internal class Program
{
    private static void Main()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        SocialNetwork socialNetwork = new SocialNetwork();

        while (true)
        {
            PrintHeader();

            Console.WriteLine("1 - Симуляція покупців у магазині");
            Console.WriteLine("2 - Симуляція поведінки користувачів у соціальній мережі");
            Console.WriteLine("0 - Вихід");
            Console.WriteLine();

            int choice = ConsoleHelper.ReadIntInRange("Ваш вибір: ", 0, 2);

            switch (choice)
            {
                case 1:
                    RunStoreMenu();
                    break;

                case 2:
                    socialNetwork.Run();
                    break;

                case 0:
                    Console.WriteLine("Роботу програми завершено.");
                    return;
            }
        }
    }

    private static void PrintHeader()
    {
        Console.WriteLine();
        Console.WriteLine("========================================================");
        Console.WriteLine("                       ФІТ-2-15");
        Console.WriteLine("         ПРАКТИЧНЕ ЗАВДАННЯ №4 З ООП");
        Console.WriteLine("          СТРУКТУРИ, КЛАСИ ТА ФУНКЦІЇ");
        Console.WriteLine("========================================================");
        Console.WriteLine();
    }

    private static void RunStoreMenu()
    {
        Store store = new Store();
        Customer[]? customers = null;
        Random random = new Random();

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("========================================================");
            Console.WriteLine("                    МАГАЗИН");
            Console.WriteLine("========================================================");
            Console.WriteLine("1 - Переглянути товари");
            Console.WriteLine("2 - Запустити нову симуляцію покупців");
            Console.WriteLine("3 - Переглянути рух товарів і залишки");
            Console.WriteLine("4 - Переглянути статистику магазину");
            Console.WriteLine("0 - Повернутися до головного меню");
            Console.WriteLine();

            int choice = ConsoleHelper.ReadIntInRange("Ваш вибір: ", 0, 4);

            switch (choice)
            {
                case 1:
                    store.ShowProducts();
                    ConsoleHelper.Pause();
                    break;

                case 2:
                    int customerCount = ConsoleHelper.ReadIntInRange(
                        "Введіть кількість покупців (від 1 до 1000): ",
                        1,
                        1000);

                    store = new Store();
                    customers = store.CreateCustomers(customerCount, random);

                    store.ShowProducts();
                    store.RunSimulation(customers, random);
                    store.ShowInventoryChanges();
                    store.ShowResults(customers);

                    ConsoleHelper.Pause();
                    break;

                case 3:
                    if (customers == null)
                    {
                        Console.WriteLine(
                            "Спочатку запустіть симуляцію покупців.");
                    }
                    else
                    {
                        store.ShowInventoryChanges();
                    }

                    ConsoleHelper.Pause();
                    break;

                case 4:
                    if (customers == null)
                    {
                        Console.WriteLine(
                            "Спочатку запустіть симуляцію покупців.");
                    }
                    else
                    {
                        store.ShowResults(customers);
                    }

                    ConsoleHelper.Pause();
                    break;

                case 0:
                    return;
            }
        }
    }
}
