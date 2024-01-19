using Practice2.Extensions;
using Practice2.UI.Base;
using static System.Console;

namespace Practice2.UI.Components;

public class TextBlock : UIElement
{
    private string[] _lines = new[] { string.Empty };

    public string Text
    {
        get => string.Join("\n", _lines);
        set
        {
            IEnumerable<string> lines = value.Split('\n').SelectMany(line => line.GetSublines(Form.MaxWidth));
            _lines = lines.ToArray();
            Width = lines.Max(line => line.Length);
            Height = lines.Count();
        }
    }

    public override void Draw(int offsetX = 0, int offsetY = 0)
    {
        int x = offsetX + X;
        int y = offsetY + Y;
        foreach (var line in _lines)
        {
            SetCursorPosition(x, y++);
            Write(line);
        }
    }
    public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
    { }
}
