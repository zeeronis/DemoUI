using UISystem;
using UnityEngine;
using Zenject;

namespace PuzzleDemo.UI
{
    public class ZenjectUIFactory : IUIFactory
    {
        private readonly DiContainer _container;

        public ZenjectUIFactory(DiContainer container)
        {
            _container = container;
        }

        public TController CreateController<TController>()
            where TController : Controller
        {
            return _container.Instantiate<TController>();
        }

        public View CreateView(View prefab, Transform parent)
        {
            return Object.Instantiate(prefab, parent);
        }
    }
}
