using System.Runtime.CompilerServices;
using UnityEngine;

namespace UISystem
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class View : MonoBehaviour
    {
        protected CanvasGroup CanvasGroup { get; private set; }
        protected Canvas Canvas { get; private set; }
        protected IViewChannel Channel { get; private set; }
        protected UIFlow UIFlow { get; private set; }

        #region Virtual Methods

        protected virtual void OnUpdate() { }

        protected virtual void OnLateUpdate() { }

        protected virtual void OnFixedUpdate() { }

        protected virtual void OnCreated() { }

        protected virtual void OnDestroying() { }

        protected virtual void OnClosing() { }

        protected virtual void OnClosed() { }

        protected virtual void OnOpening() { }

        protected virtual void OnOpened() { }

        #endregion

        #region Internal Methods

        internal void Initialize(IViewChannel channel, UIFlow flow, int order)
        {
            CanvasGroup = GetComponent<CanvasGroup>();

            if (TryGetComponent<Canvas>(out var canvas))
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = order;
                Canvas = canvas;
            }

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

        internal void Open()
        {
            SetCanvasActive(true);

            try
            {
                OnOpening();
                OnOpened();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        internal void Close()
        {
            try
            {
                OnClosing();
                SetCanvasActive(false);
                OnClosed();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        internal void Destroy()
        {
            try
            {
                OnDestroying();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        internal void TickUpdate()
        {
            try
            {
                OnUpdate();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        internal void TickLateUpdate()
        {
            try
            {
                OnLateUpdate();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        internal void TickFixedUpdate()
        {
            try
            {
                OnFixedUpdate();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetCanvasActive(bool active)
        {
            if (Canvas)
                Canvas.enabled = active;
        }

        #endregion
    }
}
