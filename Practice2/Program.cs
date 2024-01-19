using Practice2.Extensions;
using Practice2.Models;
using Practice2.UI;
using Practice2.UI.Base;
using Practice2.UI.Components;
using Practice2.UI.Factories;
using Practice2.UI.Interfaces;
using static Practice2.Models.Context;

namespace Practice2;

public class Program
{
    public static void Main(string[] args)
    {
        InitializeConsole();

        var app = new Application(InitializeUIComponents());
        app.Run();
    }

    private static Container InitializeUIComponents()
    {
        var tabControl = new TabControl(
            new()
            {
                new Tab(InitializeProductsTab().ToList())
                {
                    Header = "Товары"
                },
                new Tab(InitializeReportsTab().ToList())
                {
                    Header = "Отчёты"
                }
            });

        return tabControl;
    }

    private static IEnumerable<UIElement> InitializeReportsTab()
    {
        yield return new Button(_ => ShowProductsList(Entities.Set<Product>().ToList()))
        {
            Content = "Список товаров",
            X = 1,
            Y = 0
        };

        yield return new Button(_ => ShowTotalProductsCount(Entities.Set<Product>().ToList()))
        {
            Content = "Количество товаров",
            X = 16,
            Y = 0
        };
    }

    private static void ShowTotalProductsCount(IEnumerable<Product> products)
    {
        var form = new FormBuilder("")
                        .AddText($"Общее количество товаров: {products.Sum(p => p.Count)}")
                        .AddButton("Ок", builder =>
                        {
                            builder.OnClick(form => { form.Focused = false; });
                        })
                        .Build();

        form.CaptureKeyboard();
    }

    private static void ShowProductsList(IEnumerable<Product> products)
    {
        var form = new FormBuilder("")
                        .AddTable(products, builder =>
                        {
                            builder.AddColumn("Название", p => p.Title, stretch: true, maxWidth: 50);
                            builder.AddColumn("Количество", p => $"{p.Count:N0}", Alignment.Right);
                            builder.AddColumn("Цена", p => $"{p.Price:C2}", Alignment.Right);
                            builder.AddColumn("Сумма", p => $"{p.Count * p.Price:C2}", Alignment.Right);
                        })
                        .AddButton("Ок", builder =>
                        {
                            builder.OnClick(form => { form.Focused = false; });
                        })
                        .Build();

        form.CaptureKeyboard();
    }

    private static IEnumerable<UIElement> InitializeProductsTab()
    {
        TableController<Product> table = (TableController<Product>)new TableBuilder<Product>(Entities.Set<Product>().ToList())
                                    .AddColumn("Название", e => e.Title, Alignment.Left, stretch: true, maxWidth: 50)
                                    .AddColumn("Количество", e => $"{e.Count:N0}", Alignment.Right)
                                    .AddColumn("Цена", e => $"{e.Price:C2}", Alignment.Right)
                                    .Y(1)
                                    .OnRowSelected(args =>
                                    {
                                        var form = InitializeProductForm(args.Row.Entity);

                                        form.CaptureKeyboard();

                                        args.Table.UpdateEntities(Entities.Set<Product>().ToList());
                                    })
                                    .Build();

        yield return new Button(_ =>
        {
            var form = InitializeProductForm(null);

            form.CaptureKeyboard();

            table.Table.UpdateEntities(Entities.Set<Product>().ToList());
        })
        {
            Content = "Создать",
            X = 1,
            Y = 0
        };

        yield return new Button(_ =>
        {
            InitializeDeleteForm(table.Table.Rows.Select(row => row.Entity));

            table.Table.UpdateEntities(Entities.Set<Product>().ToList());
            table.ScrollOffset--;
            table.Selected--;
        })
        {
            Content = "Удалить",
            X = 9,
            Y = 0
        };

        yield return new Button(_ =>
        {
            InitializeChangeProductCountForm();

            table.Table.UpdateEntities(Entities.Set<Product>().ToList());
        })
        {
            Content = "Изменить количество",
            X = 17,
            Y = 0
        };

        yield return table;
    }

    private static void InitializeDeleteForm(IEnumerable<Product> products)
    {
        var form = new FormBuilder("Удалить")
                        .Model(products)
                        .AddInputField(builder =>
                        {
                            builder.Type(InputFieldType.Text);
                            builder.Label("Название");
                            builder.Width(48);
                        })
                        .AddButton("Ок", builder =>
                        {
                            builder.OnClick(form =>
                            {
                                var nameToDelete = ((InputField)form.Elements.ElementAt(0)).Value;
                                var product = Enumerable.FirstOrDefault((IEnumerable<Product>)form.Model, p => p.Title == nameToDelete);

                                if (product is null)
                                    Menu.ShowMessage("Ошибка", $"Товара с названием [{nameToDelete}] не существует");
                                else
                                {
                                    Entities.Remove(product);
                                    Entities.SaveChanges();

                                    form.Focused = false;
                                }
                            });
                        })
                        .AddButton("Отмена", builder =>
                        {
                            builder.OnClick(form =>
                            {
                                form.Focused = false;
                            });
                        })
                        .Build();

        form.CaptureKeyboard();
    }

    private static void InitializeConsole()
    {
        Console.InputEncoding = System.Text.Encoding.Unicode;
        Console.OutputEncoding = System.Text.Encoding.Unicode;

        Console.CursorVisible = false;
    }

    private static void InitializeChangeProductCountForm()
    {
        var form = new FormBuilder("Изменение количество")
                            .AddInputField(builder =>
                            {
                                builder.Type(InputFieldType.Text)
                                       .Label("Название");

                                builder.Width(48);
                            })
                            .AddInputField(builder =>
                            {
                                builder.Type(InputFieldType.Number)
                                       .Label("Новое значение");

                                builder.Width(48);
                            })
                            .AddButton("Ок", builder =>
                            {
                                builder.OnClick(form =>
                                {
                                    var productName = ((InputField)form.Elements.ElementAt(0)).Value;
                                    var product = Entities.Set<Product>().FirstOrDefault(e => e.Title == productName);

                                    if (product is null)
                                        Menu.ShowMessage("Ошибка", $"Товара с названием [{productName}] не существует");
                                    else
                                    {
                                        var countInputField = (InputField)form.Elements.ElementAt(1);
                                        if (countInputField.Value.Replace(',', ' ').Trim() == string.Empty)
                                        {
                                            Menu.ShowMessage("Ошибка", "Поле [Новое значение] не заполнено");
                                            return;
                                        }

                                        var newValue = countInputField.GetIntegerValue();
                                        if (newValue < 0 || newValue > int.MaxValue)
                                        {
                                            Menu.ShowMessage("Ошибка", "Недопустимое значение для количества");
                                            return;
                                        }

                                        product.Count = newValue;

                                        Entities.SaveChanges();
                                        form.Focused = false;
                                    }
                                });
                            })
                            .AddButton("Отмена", builder =>
                            {
                                builder.OnClick(form =>
                                {
                                    form.Focused = false;
                                });
                            })
                            .Build();

        form.CaptureKeyboard();
    }

    private static Form InitializeProductForm(Product? model)
    {
        bool isNew = model is null;
        model ??= new();

        bool Save(Form form)
        {
            InputField labelTB = (InputField)form.Elements.ElementAt(0);
            InputField countTB = (InputField)form.Elements.ElementAt(1);
            InputField PriceTB = (InputField)form.Elements.ElementAt(2);

            var newCountValue = countTB.GetIntegerValue();
            if (newCountValue < 0 || newCountValue > int.MaxValue)
            {
                Menu.ShowMessage("Ошибка", "Недопустимое значение для количества");
                return false;
            }

            var newPriceValue = (decimal)PriceTB.GetDoubleValue();
            if (newPriceValue < 0 || newPriceValue > decimal.MaxValue)
            {
                Menu.ShowMessage("Ошибка", "Недопустимое значение для цены");
                return false;
            }

            var labelValue = labelTB.Value.Trim();

            model.Title = labelValue;
            model.Count = newCountValue;
            model.Price = newPriceValue;

            var productsSet = Entities.Set<Product>();

            if (productsSet.Any(p => p.Title == labelValue && p.Id != model.Id))
            {
                Menu.ShowMessage("Ошибка", "Товар с таким название уже существует");
                return false;
            }

            if (isNew)
            {
                productsSet.Add(model);
                isNew = false;
            }


            Entities.SaveChanges();
            return true;
        }

        var form = new FormBuilder(model.Title ?? "Товар (создание)")
                                .Model(model)
                                .AddInputField(builder =>
                                {
                                    builder.Type(InputFieldType.Text);
                                    builder.Label("Название")
                                           .Value(model.Title ?? "");

                                    builder.Width(48);
                                })
                                .AddInputField(builder =>
                                {
                                    builder.Type(InputFieldType.Number);
                                    builder.Label("Количество")
                                           .Value($"{model.Count}");
                                    builder.Width(48);
                                })
                                .AddInputField(builder =>
                                {
                                    builder.Type(InputFieldType.Number);
                                    builder.Label("Цена")
                                           .Value($"{model.Price}");
                                    builder.Width(48);
                                })
                                .AddButton("Сохранить", builder =>
                                {
                                    builder.OnClick(form =>
                                    {
                                        Save(form);
                                    });
                                })
                                .AddButton("Закрыть", builder =>
                                {
                                    builder.OnClick(form =>
                                    {
                                        form.TryClose();
                                    });
                                })
                                .OnClosing(args =>
                                {
                                    var status = ShowClosingForm();
                                    switch (status)
                                    {
                                        case CloseStatus.SaveAndClose:
                                            args.Cancel = !Save(args.Form);
                                            break;

                                        case CloseStatus.CloseWithoutSave:
                                            args.Cancel = false;
                                            CancelChanges();
                                            break;
                                        case CloseStatus.DontClose:
                                            args.Cancel = true;
                                            break;
                                    }
                                })
                                .Build();

        return form;
    }

    private static CloseStatus ShowClosingForm()
    {
        var model = new { Result = CloseStatus.DontClose };

        var form = new FormBuilder("Уведомление")
                            .Model(model)
                            .AddText("Сохранить изменения?")
                            .AddButton("Да", builder =>
                            {
                                builder.OnClick(form =>
                                {
                                    form.Model = model with { Result = CloseStatus.SaveAndClose };
                                    form.Focused = false;
                                });
                            })
                            .AddButton("Нет", builder =>
                            {
                                builder.OnClick(form =>
                                {
                                    form.Model = model with { Result = CloseStatus.CloseWithoutSave };
                                    form.Focused = false;
                                });
                            })
                            .AddButton("Отмена", builder =>
                            {
                                builder.OnClick(form =>
                                {
                                    form.Model = model with { Result = CloseStatus.DontClose };
                                    form.Focused = false;
                                });
                            })
                            .Build();

        form.CaptureKeyboard();

        return form.Model.Result;
    }
}