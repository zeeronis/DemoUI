using UnityEngine;

namespace UISystem
{
    public interface IUIFactory
    {
        View CreateView(View prefab, Transform parent);
        TController CreateController<TController>()
            where TController : Controller;
    }
}
