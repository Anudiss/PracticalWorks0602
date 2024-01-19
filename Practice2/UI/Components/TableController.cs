using Practice2.Extensions;
using Practice2.UI.Base;
using static System.Console;

namespace Practice2.UI.Components;

public class TableController<TEntity> : UIElement where TEntity : class
{
    private int _selected = 0;
    private int _scrollOffset = 0;
    private int _pageItemsCount = 10;

    public int Selected
    {
        get => _selected;
        set
        {
            if (value < ScrollOffset)
                ScrollOffset--;
            else if (value >= ScrollOffset + _pageItemsCount)
                ScrollOffset++;

            _selected = Math.Clamp(value, 0, Table.Rows.Count() - 1);
        }
    }
    public int ScrollOffset
    {
        get => _scrollOffset;
        set => _scrollOffset = Math.Clamp(value, 0, Table.Rows.Count() - _pageItemsCount);
    }
    public int PageItemsCount
    {
        get => _pageItemsCount;
        set => _pageItemsCount = Math.Clamp(value, 0, 10);
    }

    public Row<TEntity> SelectedRow => this[Selected];

    public Row<TEntity> this[int index] => Table.Rows.ElementAt(index);

    public Table<TEntity> Table { get; init; }

    public TableEventHandler<TEntity>? RowSelected { get; set; }

    public TableController(Table<TEntity> table)
    {
        Table = table;
        Height = Math.Min(PageItemsCount, table.Rows.Count()) + 4;
        Width = table.Columns.Select((column, i) =>
        {
            if (column.MaxWidth == 0)
                return Math.Max(column.Header.Length, Table.Rows.Max(r => r.Values.ElementAt(i).Length));
            else
                return column.Stretch ? column.MaxWidth : Math.Min(column.MaxWidth, Math.Max(column.Header.Length, Table.Rows.Max(r => r.Values.ElementAt(i).Length)));
        }).Sum() + table.Columns.Count() - 1;
    }

    public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.UpArrow)
            Selected--;
        else if (keyInfo.Key == ConsoleKey.DownArrow)
            Selected++;
        else if (keyInfo.Key == ConsoleKey.Enter)
            RowSelected?.Invoke(new(Table, this[Selected]));
    }

    public override void Draw(int offsetX = 0, int offsetY = 0)
    {
        CursorVisible = false;
        // ┌┐└┘│─├┤┬┴┼┃
        int x = offsetX + X;
        int y = offsetY + Y;

        #region Width calculation [int[] widths, int maxWidth]
        int[] widths = Table.Columns.Select((column, i) =>
        {
            if (column.MaxWidth == 0)
                return Math.Max(column.Header.Length, Table.Rows.Max(r => r.Values.ElementAt(i).Length));
            else
                return column.Stretch ? column.MaxWidth : Math.Min(column.MaxWidth, Math.Max(column.Header.Length, Table.Rows.Max(r => r.Values.ElementAt(i).Length)));
        })
                                    .ToArray();
        int maxWidth = widths.Sum() + widths.Length;
        #endregion

        #region Header drawing
        string headerUpperLine = $"┌{string.Join("┬", widths.Select("─".Repeat))}┐";
        SetCursorPosition(x, y);
        WriteLine(headerUpperLine);

        string headerContentLine = $"│{string.Join("│", Table.Columns.Select((column, i) => column.Header.Align(Alignment.Center, widths[i])))}│";
        SetCursorPosition(x, y + 1);
        WriteLine(headerContentLine);

        string headerLowerLine = $"├{string.Join("┼", widths.Select("─".Repeat))}┤";
        SetCursorPosition(x, y + 2);
        WriteLine(headerLowerLine);
        #endregion

        #region Rows drawing
        var rows = Table.Rows.Skip(ScrollOffset).Take(PageItemsCount);
        for (int i = 0; i < rows.Count(); i++)
        {
            var row = rows.ElementAt(i);

            SetCursorPosition(x, y + i + 3);
            Write("│");
            if ((Selected - ScrollOffset) == i)
            {
                BackgroundColor = Focused ? ConsoleColor.White : ConsoleColor.DarkGray;
                ForegroundColor = Focused ? ConsoleColor.Black : ConsoleColor.White;
            }

            string rowContentLine = $"{string.Join("│", row.Values.Select((value, i) => value.Align(Table.Columns.ElementAt(i).Alignment, widths[i])))}";
            Write(rowContentLine);

            ResetColor();
            WriteLine("│");
        }
        #endregion

        #region Table last line drawing
        string tableLowerLine = $"└{string.Join("┴", widths.Select("─".Repeat))}┘";
        SetCursorPosition(x, y + 3 + PageItemsCount);
        WriteLine(tableLowerLine);
        #endregion

        #region Drawing scrollbar
        int rowsCount = Table.Rows.Count();
        if (rowsCount > PageItemsCount)
        {
            int scrollBarOffset = (int)((double)(Selected) / rowsCount * PageItemsCount + 3);
            SetCursorPosition(x + maxWidth, y + scrollBarOffset);
            Write("┃");
        }
        #endregion
    }
}
