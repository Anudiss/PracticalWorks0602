namespace Practice2.Extensions;

public static class StringExtensions
{
    public static IEnumerable<string> GetSublines(this string line, int maxWidth)
    {
        int index = 0;
        do
        {
            int length = Math.Min(maxWidth, line.Length - index);
            yield return line.Substring(index, length);
            index += length + 1;
        } while (index < line.Length - 1);
    }

    public static string CutLine(this string line, int maxWidth) =>
        maxWidth < line.Length ? string.Concat(line.AsSpan(0, maxWidth - 3), "...") : line;

    public static string Align(this string text, Alignment alignment, int maxWidth)
    {
        if (text.Length > maxWidth)
            return text.CutLine(maxWidth);

        return alignment switch
        {
            Alignment.Left => text.PadRight(maxWidth),
            Alignment.Right => text.PadLeft(maxWidth),
            Alignment.Center => text.PadLeft((maxWidth + text.Length) / 2).PadRight(maxWidth),
            _ => text
        };
    }

    public static string Repeat(this string line, int count) =>
        string.Join("", Enumerable.Repeat(line, count));

}

public enum Alignment
{
    Left,
    Right,
    Center
}