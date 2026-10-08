namespace OOP_Lab4.Services;

public static class ConsoleUI
{
    private const int Width = 72;

    public static void WriteBanner(params string[] lines)
    {
        Console.WriteLine();
        WriteColored("╔" + new string('═', Width - 2) + "╗", ConsoleColor.Cyan);

        foreach (string line in lines)
        {
            string centered = Center(line, Width - 2);
            WriteColored("║" + centered + "║", ConsoleColor.Cyan);
        }

        WriteColored("╚" + new string('═', Width - 2) + "╝", ConsoleColor.Cyan);
        Console.WriteLine();
    }

    public static void WriteSection(string title)
    {
        Console.WriteLine();
        WriteColored("┌" + new string('─', Width - 2) + "┐", ConsoleColor.DarkCyan);
        WriteColored("│ " + title.PadRight(Width - 4) + " │", ConsoleColor.DarkCyan);
        WriteColored("└" + new string('─', Width - 2) + "┘", ConsoleColor.DarkCyan);
    }

    public static void WriteMenuItem(int number, string text)
    {
        WriteColored($" [{number}] ", ConsoleColor.Yellow, false);
        Console.WriteLine(text);
    }

    public static void WriteMenuExit(string text)
    {
        WriteColored(" [0] ", ConsoleColor.DarkGray, false);
        Console.WriteLine(text);
    }

    public static void WriteSuccess(string text)
    {
        WriteColored(text, ConsoleColor.Green);
    }

    public static void WriteWarning(string text)
    {
        WriteColored(text, ConsoleColor.Yellow);
    }

    public static void WriteError(string text)
    {
        WriteColored(text, ConsoleColor.Red);
    }

    public static void WriteInfo(string text)
    {
        WriteColored(text, ConsoleColor.Cyan);
    }

    public static void WriteLabel(string label, string value)
    {
        WriteColored(label.PadRight(20), ConsoleColor.DarkCyan, false);
        Console.WriteLine(value);
    }

    public static string BuildBar(int value, int maxValue, int width = 12)
    {
        if (maxValue <= 0)
            return new string('░', width);

        double ratio = Math.Clamp((double)value / maxValue, 0, 1);
        int filled = (int)Math.Round(ratio * width);

        return new string('█', filled) +
               new string('░', width - filled);
    }

    private static string Center(string text, int width)
    {
        if (text.Length >= width)
            return text[..width];

        int left = (width - text.Length) / 2;
        int right = width - text.Length - left;

        return new string(' ', left) + text + new string(' ', right);
    }

    private static void WriteColored(
        string text,
        ConsoleColor color,
        bool newLine = true)
    {
        ConsoleColor previousColor = Console.ForegroundColor;

        try
        {
            Console.ForegroundColor = color;

            if (newLine)
                Console.WriteLine(text);
            else
                Console.Write(text);
        }
        finally
        {
            Console.ForegroundColor = previousColor;
        }
    }
}
