using System.Text;
using OOP_Lab4.Models;
using OOP_Lab4.Services;

namespace OOP_Lab4;

internal class Program
{
    private static void Main()
    {
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
                    RunStoreTask();
                    ConsoleHelper.Pause();
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

    private static void RunStoreTask()
    {
        Console.WriteLine();
        Console.WriteLine("ЗАВДАННЯ 1. СИМУЛЯЦІЯ ПОКУПЦІВ У МАГАЗИНІ");
        Console.WriteLine();

        int customerCount = ConsoleHelper.ReadPositiveInt("Введіть кількість покупців: ");

        Random random = new Random();
        Store store = new Store();
        Customer[] customers = store.CreateCustomers(customerCount, random);

        store.ShowProducts();
        store.RunSimulation(customers, random);
        store.ShowProducts();
        store.ShowResults(customers);
    }
}
