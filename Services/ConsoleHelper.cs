namespace OOP_Lab4.Services;

public static class ConsoleHelper
{
    public static int ReadPositiveInt(string message)
    {
        while (true)
        {
            Console.Write(message);

            if (int.TryParse(Console.ReadLine(), out int value) && value > 0)
                return value;

            ConsoleUI.WriteError(
                "Помилка. Введіть ціле число більше нуля.");
        }
    }

    public static int ReadIntInRange(string message, int min, int max)
    {
        while (true)
        {
            Console.Write(message);

            if (int.TryParse(Console.ReadLine(), out int value) &&
                value >= min &&
                value <= max)
            {
                return value;
            }

            ConsoleUI.WriteError(
                $"Помилка. Введіть число від {min} до {max}.");
        }
    }

    public static string ReadNonEmptyString(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? value = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();

            ConsoleUI.WriteError("Рядок не може бути порожнім.");
        }
    }

    public static void Pause()
    {
        Console.WriteLine();
        ConsoleUI.WriteInfo("Натисніть Enter, щоб продовжити...");
        Console.ReadLine();
        Console.WriteLine();
    }
}
