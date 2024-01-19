using Practice2.UI.Base;
using Practice2.UI.Components;
using Practice2.UI.Factories.Base;

namespace Practice2.UI.Factories;

public class FormBuilder
{
    private string _title;
    private List<UIElementBuilder> _elementBuilders = new();
    private FormClosing _closing;
    private object _model;
    
    public FormBuilder(string title) =>
        _title = title;

    public FormBuilder AddInputField(Action<InputFieldBuilder> inputFieldBuilder)
    {
        var builder = new InputFieldBuilder();
        inputFieldBuilder(builder);
        _elementBuilders.Add(builder);
        return this;
    }

    public FormBuilder AddText(string text)
    {
        var builder = new TextBlockBuilder(text);
        _elementBuilders.Add(builder);
        return this;
    }

    public FormBuilder AddTable<TEntity>(IEnumerable<TEntity> entities, Action<TableBuilder<TEntity>> tableBuilder) where TEntity : class
    {
        var builder = new TableBuilder<TEntity>(entities);
        tableBuilder(builder);
        _elementBuilders.Add(builder);
        return this;
    }

    public FormBuilder AddButton(string content, Action<ButtonBuilder> buttonBuilder)
    {
        var builder = new ButtonBuilder(content);
        buttonBuilder(builder);
        _elementBuilders.Add(builder);
        return this;
    }

    public FormBuilder OnClosing(FormClosing onClosing)
    {
        _closing = onClosing;
        return this;
    }

    public FormBuilder Model(object model)
    {
        _model = model;
        return this;
    }

    public Form Build()
    {
        UIElement[] elements = _elementBuilders.Select(builder => builder.Build()).ToArray();
        for (int i = 1; i < elements.Length; i++)
        {
            var previous = elements[i - 1];
            var current = elements[i];

            if (current is Button)
            {
                current.X = previous.X - current.Width - 1;
                current.Y = previous.Y + ((previous is not Button) ? previous.Height : 0);
            }
            else
                current.Y = previous.Y + previous.Height;
        }

        Form form = new(elements.ToList())
        {
            Title = _title,
            Closing = _closing,
            Model = _model,
        };

        elements.Where(e => e is Button).Cast<Button>().ToList().ForEach(button => button.Form = form);

        return form;
    }
}
