using Practice2.Extensions;
using Practice2.UI.Components;
using Practice2.UI.Interfaces;
using static System.Console;

namespace Practice2.UI.Base;

public delegate void FormClosing(CancelFormEventHandlerArgs args);

public class Form : Container
{
    public const int MinWidth = 15;
    public const int MaxWidth = 110;

    public required dynamic Model { get; set; }

    public required string Title { get; set; }

    public FormClosing Closing { get; init; }

    public Form(List<UIElement> elements) : base(elements) =>
        MeasureSize();

    public override void Add(UIElement element)
    {
        _elements.Add(element);
        MeasureSize();
    }

    private void MeasureSize()
    {
        Width = Math.Clamp(_elements.Max(e => e.X + e.Width) - _elements.Where(e => e is not Button).Min(e => e.X) + 2, MinWidth, MaxWidth);
        Height = _elements.Where(e => e is not Button).Sum(e => e.Height) + 1;
    }

    public override void Draw(int offsetX = 0, int offsetY = 0)
    {
        CursorVisible = false;
        // ┌┐└┘│─├┤┬┴┼┃
        int x = (WindowWidth - Width) / 2;
        int y = (WindowHeight - Height) / 2;

        string upperLine = $"┌{Title.PadLeft((Width + Title.Length) / 2, '─').PadRight(Width, '─')}┐";
        SetCursorPosition(x, y);
        Write(upperLine);

        for (int yy = 0; yy < Height; yy++)
        {
            SetCursorPosition(x, y + yy + 1);
            Write("│");

            SetCursorPosition(x + Width + 1, y + yy + 1);
            Write("│");
        }

        string lowerLine = $"└{"─".Repeat(Width)}┘";
        SetCursorPosition(x, y + Height);
        Write(lowerLine);

        for (int i = 0; i < _elements.Count; i++)
        {
            var element = _elements[i];
            element.Draw(x + 1 + ((element is Button) ? Width : 0), y + 1);
        }

        if (FocusedElement is InputField)
            FocusedElement.Draw(x + 1, y + 1);
    }

    public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Tab)
            FocusedElementIndex += keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? -1 : 1;

        if (keyInfo.Key == ConsoleKey.Escape)
            TryClose();

        if (FocusedElement is not null)
            FocusedElement.OnKeyPressed(keyInfo);
    }

    public void TryClose()
    {
        CancelFormEventHandlerArgs args = new(this);
        Closing?.Invoke(args);

        if (!args.Cancel)
            Focused = false;
    }

    public void CaptureKeyboard()
    {
        Focused = true;

        do
        {
            Draw();

            var keyInfo = ReadKey(true);
            OnKeyPressed(keyInfo);

        } while (Focused);
    }
}

public enum CloseStatus
{
    SaveAndClose,
    CloseWithoutSave,
    DontClose
}

public class FormEventHandlerArgs
{
    public Form Form { get; init; }

    public FormEventHandlerArgs(Form form) =>
        Form = form;
}

public class CancelFormEventHandlerArgs : FormEventHandlerArgs
{
    public bool Cancel { get; set; }

    public CancelFormEventHandlerArgs(Form form) : base(form)
    {
    }
}
