using Microsoft.IdentityModel.Tokens;
using Practice2.UI.Base;
using Practice2.UI.Components;

namespace Practice2.UI.Interfaces;

public abstract class Container : UIElement
{
    protected List<UIElement> _elements;
    private int _focusedElementIndex = 0;
    private UIElement[] _focusableElements;

    public int FocusedElementIndex
    {
        get => _focusedElementIndex;
        set
        {
            _focusedElementIndex = value < 0 ? _focusableElements.Length - value - 2 :
                                        value > _focusableElements.Length - 1 ? value - _focusableElements.Length : value;

            Focus(_focusedElementIndex);
        }
    }
    public UIElement? FocusedElement => _focusableElements.IsNullOrEmpty() ? null : _focusableElements[FocusedElementIndex];

    public IEnumerable<UIElement> Elements => _elements;

    public Container(List<UIElement> elements)
    {
        _elements = elements.ToList();
        _focusableElements = elements.Where(e => e is not TextBlock).ToArray();

        Focus(_focusableElements.FirstOrDefault());
    }

    public abstract void Add(UIElement element);

    protected void Focus(UIElement? element)
    {
        if (element is null)
            return;

        foreach (var e in Elements)
            e.Focused = false;

        element.Focused = true;
    }

    private void Focus(int index)
    {
        _focusableElements.ToList().ForEach(e => e.Focused = false);
        _focusableElements[index].Focused = true;
    }
}
