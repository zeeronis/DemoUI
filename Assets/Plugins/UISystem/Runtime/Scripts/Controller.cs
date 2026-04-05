using UnityEngine;

namespace UISystem
{
    public abstract class Controller
    {
        protected IControllerChannel Channel { get; private set; }
        protected UIFlow UIFlow { get; private set; }
        protected string WindowId { get; private set; }

        internal void Initialize(IControllerChannel channel, UIFlow flow, string windowId)
        {
            WindowId = windowId;
            Channel = channel;
            UIFlow = flow;
        }

        internal void Created()
        {
            try
            {
                OnCreated();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        protected void OpenView()
        {
            UIFlow.OpenView(WindowId);
        }

        protected void OpenView<TData>(TData data)
        {
            UIFlow.OpenView(WindowId, data);
        }

        protected void CloseView()
        {
            UIFlow.CloseView(WindowId);
        }

        protected internal virtual void OnOpenWindowRequest() => OpenView();
        protected internal virtual void OnCloseWindowRequest() => CloseView();
        protected virtual void OnCreated() { }
    }
}
