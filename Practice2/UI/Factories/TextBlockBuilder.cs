using Practice2.UI.Base;
using Practice2.UI.Components;
using Practice2.UI.Factories.Base;

namespace Practice2.UI.Factories;

public class TextBlockBuilder : UIElementBuilder
{
    private string _text;

    public TextBlockBuilder(string text) =>
        _text = text;

    public override UIElement Build() =>
        new TextBlock()
        {
            Text = _text
        };
}
