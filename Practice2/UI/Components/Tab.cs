using Practice2.UI.Base;
using Practice2.UI.Interfaces;

namespace Practice2.UI.Components;

public class Tab : Container
{
    private string _header;

    public string Header
    {
        get => _header;
        set
        {
            _header = value;
            Width = _header.Length + 2;
        }
    }

    public Tab(List<UIElement> elements) : base(elements)
    {
        Height = 2;
    }

    public override void Add(UIElement element) => _elements.Add(element);
    public override void Draw(int offsetX = 0, int offsetY = 0)
    {
        foreach (var element in _elements)
            element.Draw(offsetX, offsetY);
    }
    public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Tab)
            FocusedElementIndex += keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? -1 : 1;

        FocusedElement?.OnKeyPressed(keyInfo);
    }
}
