namespace OOP_Lab4.Services;

public static class ConsoleHelper
{
    public static int ReadPositiveInt(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();

            if (input == null)
                throw new EndOfStreamException("Ввід завершено.");

            if (int.TryParse(input, out int value) && value > 0)
                return value;

            ConsoleUI.WriteError("❌ Введіть ціле число більше нуля.");
        }
    }

    public static int ReadIntInRange(string message, int min, int max)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();

            if (input == null)
                throw new EndOfStreamException("Ввід завершено.");

            if (int.TryParse(input, out int value) &&
                value >= min &&
                value <= max)
            {
                return value;
            }

            ConsoleUI.WriteError($"❌ Введіть число від {min} до {max}.");
        }
    }

    public static string ReadNonEmptyString(string message)
    {
        return ReadText(message, 280);
    }

    public static string ReadText(string message, int maxLength)
    {
        if (maxLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxLength));

        while (true)
        {
            Console.Write(message);
            string? value = Console.ReadLine();

            if (value == null)
                throw new EndOfStreamException("Ввід завершено.");

            value = value.Trim();

            if (string.IsNullOrWhiteSpace(value))
            {
                ConsoleUI.WriteError("❌ Текст не може бути порожнім.");
                continue;
            }

            if (value.Length > maxLength)
            {
                ConsoleUI.WriteError(
                    $"❌ Максимальна довжина — {maxLength} символів.");
                continue;
            }

            if (value.Any(char.IsControl))
            {
                ConsoleUI.WriteError(
                    "❌ Керувальні символи в тексті заборонені.");
                continue;
            }

            return value;
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
