using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace UISystem
{
    internal sealed class WindowsComposer
    {
        public interface IControllerComposer
        {
            Controller CreateController(IUIFactory factory);
        }

        public sealed class ControllerComposer<TController> : IControllerComposer
            where TController : Controller
        {
            [Preserve]
            public Controller CreateController(IUIFactory factory)
            {
                return factory.CreateController<TController>();
            }
        }

        private readonly Dictionary<string, IControllerComposer> _controllerComposerById = new(System.StringComparer.Ordinal);
        private readonly Dictionary<string, WindowConfig> _windowConfigById = new(System.StringComparer.Ordinal);

        public void Bind<TController>(string windowId)
            where TController : Controller
        {
            _controllerComposerById[windowId] = new ControllerComposer<TController>();
        }

        public void MapWindowConfigs(List<WindowConfig> windowConfigs)
        {
            _windowConfigById.Clear();

            for (int i = 0; i < windowConfigs.Count; i++)
            {
                WindowConfig item = windowConfigs[i];

                if (string.IsNullOrWhiteSpace(item.WindowId) || item.ViewPrefab == null)
                {
                    Debug.LogWarning($"[UISystem] Config index {i} - Prefab is null or window id is empty.");
                    continue;
                }

                _windowConfigById[item.WindowId] = item;
            }
        }

        public RuntimeWindowContext CreateWindow(string windowId, IUIFactory factory, Transform parent, UIFlow uiFlow)
        {
            if (!_windowConfigById.TryGetValue(windowId, out var config))
            {
                Debug.LogError($"[UISystem] {windowId} - Window config not found.");
                return null;
            }

            if (!TryCreateView(config, factory, parent, out var view))
                return null;

            if (!TryCreateController(windowId, factory, out var controller))
                return null;

            var context = new RuntimeWindowContext();
            context.View = view;
            context.Controller = controller;
            context.Channel = new Channel(
                context.Controller.GetType().Name,
                context.View.GetType().Name);
            context.IsOpen = false;

            context.Controller.Initialize(context.Channel, uiFlow, windowId);
            context.View.Initialize(context.Channel, uiFlow, config.Order);
            context.View.SetCanvasActive(false);

            return context;
        }

        private bool TryCreateController(string windowId, IUIFactory factory, out Controller controller)
        {
            controller = default;
            if (!_controllerComposerById.TryGetValue(windowId, out var composer))
            {
                Debug.LogError($"[UISystem] {windowId} - Controller not binded.");
                return false;
            }

            controller = composer?.CreateController(factory);
            if (controller == null)
            {
                Debug.LogError($"[UISystem] {windowId} - Controller is not created in factory.");
                return false;
            }

            return true;
        }
        
        private bool TryCreateView(WindowConfig config, IUIFactory factory, Transform parent, out View view)
        {
            view = default;
            if (config.ViewPrefab == null)
            {
                Debug.LogError($"[UISystem] {config.WindowId} - Prefab is null.");
                return false;
            }

            view = factory.CreateView(config.ViewPrefab, parent);
            if (view == null)
            {
                Debug.LogError($"[UISystem] {config.WindowId} - View is not created in factory.");
                return false;
            }

            return true;
        }
    }
}
