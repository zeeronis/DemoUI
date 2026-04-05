using PuzzleDemo.UI;
using PuzzleDemo.UI.Types;
using UISystem;
using Zenject;

namespace PuzzleDemo
{
    public class UIInitializer : IInitializable
    {
        private readonly UIManager _uiManager;

        public UIInitializer(UIManager uiManager)
        {
            _uiManager = uiManager;
        }

        public void Initialize()
        {
            // Window binding can be called in other features before the UI is initialized.
            _uiManager.BindControllerToWindow<PuzzlePreviewController, PuzzlePreview>();
            _uiManager.BindControllerToWindow<GalleryController, Gallery>();
            _uiManager.BindControllerToWindow<MenuController, Menu>();
            
            _uiManager.Initialize();
        }
    }
}
