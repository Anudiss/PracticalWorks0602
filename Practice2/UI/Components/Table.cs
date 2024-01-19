using Practice2.Extensions;

namespace Practice2.UI.Components;

public delegate void TableEventHandler<TEntity>(TableEventHandlerArgs<TEntity> agrs) where TEntity : class;

public class Table<TEntity> where TEntity : class
{
    private readonly List<Column<TEntity>> _columns = new List<Column<TEntity>>();
    private IEnumerable<Row<TEntity>> _rows = default!;

    public IEnumerable<Column<TEntity>> Columns => _columns;
    public IEnumerable<Row<TEntity>> Rows => _rows;

    public Table(List<Column<TEntity>> columns, IEnumerable<TEntity> entities)
    {
        _columns = columns;
        _rows = entities.Select(entity => new Row<TEntity>(entity, columns));
    }

    public void UpdateEntities(IEnumerable<TEntity> entities) =>
        _rows = entities.Select(entity => new Row<TEntity>(entity, _columns));
}

public class Row<TEntity> where TEntity : class
{
    private TEntity _entity;
    private string[] _values;

    public IEnumerable<string> Values => _values;
    public TEntity Entity => _entity;

    public Row(TEntity entity, IEnumerable<Column<TEntity>> columns)
    {
        _entity = entity;
        _values = columns.Select(column => column.Getter(entity)).ToArray();
    }
}

public class Column<TEntity> where TEntity : class
{
    public string Header { get; private set; }
    public Alignment Alignment { get; init; } = Alignment.Left;
    public bool Stretch { get; set; } = false;
    public int MaxWidth { get; set; }

    public Func<TEntity, string> Getter { get; private set; }

    public Column(string header, Func<TEntity, string> getter)
    {
        Header = header;
        Getter = getter;
    }
}

public class TableEventHandlerArgs<TEntity> where TEntity : class
{
    public Table<TEntity> Table { get; init; }
    public Row<TEntity> Row { get; init; }

    public TableEventHandlerArgs(Table<TEntity> table, Row<TEntity> row)
    {
        Table = table;
        Row = row;
    }
}
