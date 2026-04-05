using UnityEngine;

namespace UISystem
{
    public sealed class DefaultUIFactory : IUIFactory
    {
        public View CreateView(View prefab, Transform parent)
        {
            if (prefab == null)
                return null;

            return Object.Instantiate(prefab, parent);
        }

        public T CreateController<T>()
            where T : Controller
        {
            // Not AOT friendly
            return System.Activator.CreateInstance<T>();
        }
    }
}
