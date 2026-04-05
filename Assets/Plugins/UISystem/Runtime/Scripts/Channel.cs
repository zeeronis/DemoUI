using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UISystem
{
    public sealed class Channel : IViewChannel, IControllerChannel
    {
        private readonly string _controllerName;
        private readonly string _viewName;

        private readonly Dictionary<Type, Delegate> _intentListeners = new();
        private readonly Dictionary<Type, Delegate> _stateListeners = new();

        public Channel(string controllerName, string viewName)
        {
            _controllerName = controllerName;
            _viewName = viewName;
        }

        #region IViewChannel and IControllerChannel implementations

        void IViewChannel.SubscribeToState<T>(Action<T> handler) => SubscribeToState(handler);
        void IViewChannel.UnsubscribeFromState<T>() => UnsubscribeFromState<T>();
        void IViewChannel.SendIntent<T>(T intent) => SendIntent(intent);

        void IControllerChannel.SubscribeToIntent<T>(Action<T> handler) => SubscribeToIntent(handler);
        void IControllerChannel.UnsubscribeFromIntent<T>() => UnsubscribeFromIntent<T>();
        void IControllerChannel.SendState<T>(T state) => SendState(state);

        #endregion

        public void SendIntent<T>(T intent)
        {
            if (!TryInvoke(_intentListeners, intent))
                Debug.LogWarning($"[UISystem] Controller '{_controllerName}' has no listener for '{typeof(T).Name}' type.");
        }

        public void SendState<T>(T state)
        {
            if (!TryInvoke(_stateListeners, state))
                Debug.LogWarning($"[UISystem] View '{_viewName}' has no listener for '{typeof(T).Name}' type.");
        }

        public void SubscribeToIntent<T>(Action<T> handler)
            => SubscribeOrOverrideExists(_intentListeners, handler, _controllerName);

        public void SubscribeToState<T>(Action<T> handler)
            => SubscribeOrOverrideExists(_stateListeners, handler, _viewName);

        public void UnsubscribeFromIntent<T>()
            => UnsubscribeAllOfType<T>(_intentListeners);

        public void UnsubscribeFromState<T>()
            => UnsubscribeAllOfType<T>(_stateListeners);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SubscribeOrOverrideExists<T>(Dictionary<Type, Delegate> map, Action<T> handler, string name)
        {
            if (handler == null)
                return;

            var dataType = typeof(T);
            if (map.TryGetValue(dataType, out var existing)
                && existing != null
                && !ReferenceEquals(existing, handler))
            {
                Debug.LogWarning($"[UISystem] '{name}' is overwriting subscribe for '{dataType.Name}' type.");
            }

            map[dataType] = handler;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnsubscribeAllOfType<T>(Dictionary<Type, Delegate> map)
        {
            map.Remove(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryInvoke<T>(Dictionary<Type, Delegate> map, T data)
        {
            if (!map.TryGetValue(typeof(T), out var rawAction) || rawAction is not Action<T> action)
                return false;

            action.Invoke(data);
            return true;
        }
    }
}

