using Practice2.UI.Interfaces;

namespace Practice2.UI.Base;

public abstract class UIElement : IFocusable
{
    private bool _focused;

    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool Focused
    {
        get => _focused;
        set
        {
            _focused = value;
            if (value)
                OnFocused();
            else
                OnUnfocused();
        }
    }

    public virtual void OnFocused()
    { }
    
    public virtual void OnUnfocused()
    { }

    public abstract void OnKeyPressed(ConsoleKeyInfo keyInfo);
    public abstract void Draw(int offsetX = 0, int offsetY = 0);
}
