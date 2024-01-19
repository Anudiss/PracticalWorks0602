using Practice2.Extensions;
using Practice2.UI.Base;
using Practice2.UI.Factories;
using static System.Console;

namespace Practice2.UI.Components;

public class InputField : UIElement
{
    private int _cursorPosition;
    private int _cursorOffset;
    private string _label;

    public InputFieldType Type { get; init; }
    public required string Label
    {
        get => _label;
        set
        {
            _label = value;
            Width = Math.Max(value.Length, Value?.Length ?? 0);
        }
    }
    public string Value { get; set; } = string.Empty;

    public int CursorOffset
    {
        get => _cursorOffset;
        set => _cursorOffset = Math.Clamp(value, 0, Math.Max(0, Value.Length - Width));
    }
    public int CursorPosition
    {
        get => _cursorPosition;
        set
        {
            if (value < CursorOffset)
                CursorOffset--;
            else if (value >= CursorOffset + Width)
                CursorOffset++;

            _cursorPosition = Math.Clamp(value, 0, Value.Length);
        }
    }

    public InputField(InputFieldType type)
    {
        Type = type;
        Height = 3;
    }

    public override void Draw(int offsetX = 0, int offsetY = 0)
    {
        // ┌┐└┘│─├┤┬┴┼┃╭╮╯╰
        CursorVisible = false;
        int x = offsetX + X;
        int y = offsetY + Y;

        string upperLine = $"╭{Label.PadRight(Width, '─')}╮";
        SetCursorPosition(x, y);
        WriteLine(upperLine);

        string contentLine = $"│{Value.Substring(CursorOffset, Math.Min(Value.Length, Width)).PadRight(Width)}│";
        SetCursorPosition(x, y + 1);
        WriteLine(contentLine);

        string lowerLine = $"╰{"─".Repeat(Width)}╯";
        SetCursorPosition(x, y + 2);
        WriteLine(lowerLine);

        if (Focused)
        {
            CursorVisible = true;
            SetCursorPosition(x + 1 + CursorPosition - CursorOffset, y + 1);
        }
    }

    public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.LeftArrow)
            CursorPosition--;
        else if (keyInfo.Key == ConsoleKey.RightArrow)
            CursorPosition++;
        else if (keyInfo.Key == ConsoleKey.Backspace)
        {
            if (CursorPosition > 0)
            {
                Value = Value.Remove(--CursorPosition, 1);
                CursorOffset--;
            }
        }
        else if (keyInfo.Key == ConsoleKey.Delete)
        {
            if (CursorPosition < Value.Length)
            {
                Value = Value.Remove(CursorPosition, 1);
                CursorOffset--;
            }
        }
        else
        {
            var charToInsert = keyInfo.KeyChar;
            if (CanInsertSymbol(charToInsert))
            {
                Value = Value.Insert(CursorPosition, $"{charToInsert}");
                CursorPosition++;
            }
        }
    }

    private bool CanInsertSymbol(char charToInsert)
    {
        if (Type == InputFieldType.Text)
            return char.IsLetterOrDigit(charToInsert) || char.IsPunctuation(charToInsert) || charToInsert == ' ';
        else if (Type == InputFieldType.Number)
            return char.IsDigit(charToInsert) || (charToInsert == ',' && !Value.Any(e => e.Equals(',')));
        return false;
    }

    public int GetIntegerValue()
    {
        if (double.TryParse(Value, out var value))
            return (int)value;
        return default;
    }
    public double GetDoubleValue()
    {
        if (double.TryParse(Value, out var value))
            return value;
        return default;
    }
}

public enum InputFieldType
{
    Text,
    Number
}