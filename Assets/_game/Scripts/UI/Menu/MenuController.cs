using PuzzleDemo.UI.MenuMessages;
using PuzzleDemo.UI.Types;
using UISystem;

namespace PuzzleDemo.UI
{
    public class MenuController : Controller
    {
        private readonly CurrencyService _currencyService;
        private string _currentTabId = default(Gallery).Id;


        public MenuController(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        protected override void OnCreated()
        {
            Channel.SubscribeToIntent<SelectTabIntent>(OnSelectTabIntent);
        }

        protected override void OnOpenWindowRequest()
        {
            OpenView(new MenuState
            {
                currencyPack = _currencyService.GetBalance(CurrencyType.Soft),
            });

            OpenTab(_currentTabId);
        }

        private void OnSelectTabIntent(SelectTabIntent data)
        {
            OpenTab(data.windowId);
        }

        private void OpenTab(string windowId)
        {
            UIFlow.Close(_currentTabId);
            _currentTabId = windowId;
            UIFlow.Open(_currentTabId);
        }
    }
}
