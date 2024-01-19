using Practice2.UI;
using Practice2.UI.Interfaces;

namespace Practice2;

public class Application
{
    private readonly Container _container;

    public Application(Container container) =>
        _container = container;

    public void Run() =>
        CaptureKeyInput();

    private void CaptureKeyInput()
    {
        Console.CursorVisible = false;

        while (true)
        {
            Draw();

            var keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.F1)
                Menu.ShowMessage("Справка", "Tab (SHIFT) - перемещение между элементами\n CTRL + ← → - перемещение между вкладками\n    ↑  ↓    - перещемение в таблице\n     ESC    - закрыть форму");

            _container.OnKeyPressed(keyInfo);
        }
    }

    private void Draw()
    {
        string reference = "Справка F1";

        Console.Clear();
        Console.SetCursorPosition(Console.WindowWidth - reference.Length - 1, 1);
        Console.WriteLine(reference);

        _container.Draw();
    }
}
