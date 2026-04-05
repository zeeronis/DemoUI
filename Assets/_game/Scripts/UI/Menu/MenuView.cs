using PuzzleDemo.UI.MenuMessages;
using PuzzleDemo.UI.Types;
using UISystem;
using UnityEngine;

namespace PuzzleDemo.UI
{
    public class MenuView : View
    {
        [Header("References")]
        [SerializeField] private CurrencyPanel _currencyPanel;

        protected override void OnCreated()
        {
            Channel.SubscribeToState<MenuState>(OnMenuState);
        }

        private void OnMenuState(MenuState state)
        {
            if (_currencyPanel)
                _currencyPanel.Initialize(state.currencyPack);
        }

        private void Tab_OnClick(string windowId)
        {
            Channel.SendIntent(new SelectTabIntent { windowId = windowId });
        }
    }
}
