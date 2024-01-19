using Practice2.Extensions;
using Practice2.UI.Base;
using Practice2.UI.Components;
using Practice2.UI.Factories.Base;

namespace Practice2.UI.Factories;

public class TableBuilder<TEntity> : UIElementBuilder where TEntity : class
{
    private List<Column<TEntity>> _columns = new();
    private IEnumerable<TEntity> _entites;
    private TableEventHandler<TEntity>? _rowSelected = null;
    private int _y;

    public TableBuilder(IEnumerable<TEntity> entities) =>
        _entites = entities;

    public TableBuilder<TEntity> OnRowSelected(TableEventHandler<TEntity> rowSelected)
    {
        _rowSelected = rowSelected;
        return this;
    }

    public TableBuilder<TEntity> Y(int y)
    {
        _y = y;
        return this;
    }

    public TableBuilder<TEntity> AddColumn(string header, Func<TEntity, string> getter, Alignment alignment = Alignment.Left,
                                                                                        bool stretch = false,
                                                                                        int maxWidth = 0)
    {
        _columns.Add(new(header, getter)
        {
            Alignment = alignment,
            Stretch = stretch,
            MaxWidth = maxWidth
        });
        return this;
    }

    public override UIElement Build() =>
        new TableController<TEntity>(new Table<TEntity>(_columns, _entites))
        {
            Y = _y,
            RowSelected = _rowSelected
        };
}
