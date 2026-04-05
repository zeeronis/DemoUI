using PuzzleDemo.UI.Types;
using UISystem;

namespace PuzzleDemo
{
    // TODO: Replace it to state machine, i will not implement it in demo project. You can use any framework or write one yourself.
    public class MenuLifecycle : Zenject.IInitializable
    {
        private readonly UIManager _ui;

        public MenuLifecycle(UIManager ui)
        {
            _ui = ui;
        }

        public void Initialize()
        {
            _ui.Flow.Open<Menu>();
        }
    }
}
