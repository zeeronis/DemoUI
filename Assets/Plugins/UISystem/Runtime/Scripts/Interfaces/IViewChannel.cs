using System;

namespace UISystem
{
    public interface IViewChannel
    {
        void SendIntent<T>(T intent);
        void SubscribeToState<T>(Action<T> handler);
        void UnsubscribeFromState<T>();
    }
}
