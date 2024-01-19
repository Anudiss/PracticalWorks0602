using Practice2.UI.Base;
using static System.Console;

namespace Practice2.UI.Components;

public class Button : UIElement
{
    private Form _form;
    private readonly Action<Form>? _click;
    private string _content;

    public Form Form
    {
        get => _form;
        set => _form = value;
    }
    
    public required string Content
    {
        get => _content;
        set
        {
            _content = value;
            Width = value.Length;
        }
    }

    public Button(Action<Form>? click)
    {
        _click = click;
        Height = 1;
    }

    public override void Draw(int offsetX = 0, int offsetY = 0)
    {
        // ┌┐└┘│─├┤┬┴┼┃
        SetCursorPosition(offsetX + X, offsetY + Y);
        if (Focused)
        {
            BackgroundColor = ConsoleColor.White;
            ForegroundColor = ConsoleColor.Black;
        }

        Write(Content);

        ResetColor();
    }

    public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Enter)
            _click?.Invoke(_form);
    }
}
