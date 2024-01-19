using Practice2.UI.Base;
using Practice2.UI.Components;
using Practice2.UI.Factories.Base;

namespace Practice2.UI.Factories;

public class ButtonBuilder : UIElementBuilder
{
    private string _content;
    private Action<Form>? _click;

    public ButtonBuilder(string content)
    {
        _content = content;
    }

    public ButtonBuilder OnClick(Action<Form> click)
    {
        _click = click;
        return this;
    }

    public override UIElement Build() => new Button(_click)
    {
        Content = _content
    };
}
