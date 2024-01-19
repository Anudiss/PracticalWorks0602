using Practice2.Extensions;
using Practice2.UI.Base;
using Practice2.UI.Interfaces;
using static System.Console;

namespace Practice2.UI.Components;

public class TabControl : Container
{
    public TabControl(List<UIElement> elements) : base(elements)
    {
    }

    public override void Add(UIElement element) => _elements.Add(element);

    public override void Draw(int offsetX = 0, int offsetY = 0)
    {
        // ┌┐└┘│─├┤┬┴┼┃
        int headerWidth = _elements.Sum(e => e.Width);

        string headerFirstLine = $"┌{string.Join("┬", _elements.Select(e => "─".Repeat(e.Width)))}┐";
        SetCursorPosition(0, 0);
        WriteLine(headerFirstLine);

        SetCursorPosition(0, 1);
        Write("│");
        for (int i = 0; i < _elements.Count; i++)
        {
            var tab = (Tab)_elements[i];
            string tabHeader = $"{tab.Header}";

            Write(" ");

            if (i == FocusedElementIndex)
            {
                BackgroundColor = ConsoleColor.White;
                ForegroundColor = ConsoleColor.Black;
            }

            Write(tabHeader);

            ResetColor();

            Write(" │");
        };

        string headerLastLine = $"├{string.Join("┴", _elements.Select(e => "─".Repeat(e.Width)))}┴";
        SetCursorPosition(0, 2);
        WriteLine(headerLastLine + "─".Repeat(WindowWidth - headerLastLine.Length - 1) + "┐");

        for (int y = 3; y < WindowHeight - 1; y++)
        {
            SetCursorPosition(0, y);
            Write("│");

            SetCursorPosition(WindowWidth - 1, y);
            Write("│");
        }

        string lowerLine = $"└{"─".Repeat(WindowWidth - 2)}┘";
        SetCursorPosition(0, WindowHeight - 1);
        Write(lowerLine);

        FocusedElement?.Draw(1, 3);
    }

    public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.LeftArrow && keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
            FocusedElementIndex--;
        else if (keyInfo.Key == ConsoleKey.RightArrow && keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
            FocusedElementIndex++;

        FocusedElement?.OnKeyPressed(keyInfo);
    }
}
