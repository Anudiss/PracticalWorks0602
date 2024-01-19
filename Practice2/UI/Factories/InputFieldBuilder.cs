using Practice2.UI.Components;
using Practice2.UI.Factories.Base;

namespace Practice2.UI.Factories;

public class InputFieldBuilder : UIElementBuilder
{
    private string _label = string.Empty;
    private string _value = string.Empty;
    private int _width = 10;
    private InputFieldType _fieldType;

    public InputFieldBuilder Label(string label)
    {
        _label = label;
        return this;
    }

    public InputFieldBuilder Value(string value)
    {
        _value = value;
        return this;
    }

    public InputFieldBuilder Type(InputFieldType type)
    {
        _fieldType = type;
        return this;
    }

    public InputFieldBuilder Width(int width)
    {
        _width = width;
        return this;
    }

    public override InputField Build() => new(_fieldType)
    {
        Label = _label,
        Value = _value,
        Width = _width
    };
}
