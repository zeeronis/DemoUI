using PuzzleDemo.UI;
using UISystem;
using UnityEngine;
using Zenject;

namespace PuzzleDemo
{
    // In demo project i will use only one installer for all services and dependencies.
    // Just for simplicity.
    public class ExampleInstaller : MonoInstaller
    {
        [SerializeField] private UIManager _uiManagerPrefab;

        public override void InstallBindings()
        {
            // Dependencies
            Container.BindInterfacesAndSelfTo<MenuLifecycle>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<UIInitializer>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<UIManager>()
                .FromComponentInNewPrefab(_uiManagerPrefab)
                .AsSingle()
                .OnInstantiated<UIManager>(OnUIManagerInstantiated)
                .NonLazy();

            Container.BindInterfacesAndSelfTo<CurrencyService>()
                .AsSingle()
                .NonLazy();

            // Initialization order
            Container.BindInitializableExecutionOrder<UIInitializer>(1);
            Container.BindInitializableExecutionOrder<MenuLifecycle>(2);
        }

        private void OnUIManagerInstantiated(InjectContext context, UIManager uiManager)
        {
            uiManager.Factory = new ZenjectUIFactory(Container);
        }
    }
}
