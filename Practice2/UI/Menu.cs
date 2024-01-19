using Practice2.Extensions;

namespace Practice2.UI;

public static class Menu
{
    public static readonly int MinMenuWidth = 10;
    public static readonly int MaxMenuWidth = 48;

    public static readonly int MinMenuHeight = 3;
    public static readonly int MaxMenuHeight = 18;

    public static void ShowMessage(string title, string message)
    {
        // ┌┐└┘│─├┤┬┴┼┃
        IEnumerable<string> lines = message.Split('\n').SelectMany(line => line.GetSublines(MaxMenuWidth));

        int maxWidthMenu = Math.Max(MinMenuWidth, Math.Max(title.Length, lines.Max(e => e.Length)));
        int maxHeightMenu = lines.Count() + 1;

        int x = (Console.WindowWidth - maxWidthMenu) / 2;
        int y = (Console.WindowHeight - maxHeightMenu) / 2;

        string titleUpperLine = $"┌{"─".Repeat(maxWidthMenu)}┐";
        Console.SetCursorPosition(x, y);
        Console.WriteLine(titleUpperLine);

        string titleLine = $"│{title.PadLeft((maxWidthMenu + title.Length) / 2).PadRight(maxWidthMenu)}│";
        Console.SetCursorPosition(x, y + 1);
        Console.WriteLine(titleLine);

        string titleLowerLine = $"├{"─".Repeat(maxWidthMenu)}┤";
        Console.SetCursorPosition(x, y + 2);
        Console.WriteLine(titleLowerLine);

        for (int yy = 0; yy < lines.Count(); yy++)
        {
            var content = lines.ElementAt(yy);

            string contentLine = $"│{content.PadRight(maxWidthMenu)}│";
            Console.SetCursorPosition(x, y + yy + 3);
            Console.WriteLine(contentLine);
        }

        string basementLine = $"└{"─".Repeat(maxWidthMenu)}┘";
        Console.SetCursorPosition(x, y + lines.Count() + 3);
        Console.WriteLine(basementLine);

        Console.ReadKey(true);
    }
}