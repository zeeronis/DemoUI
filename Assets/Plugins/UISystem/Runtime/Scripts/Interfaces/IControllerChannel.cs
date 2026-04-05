using System;

namespace UISystem
{
    public interface IControllerChannel
    {
        void SendState<T>(T state);
        void SubscribeToIntent<T>(Action<T> handler);
        void UnsubscribeFromIntent<T>();
    }
}
