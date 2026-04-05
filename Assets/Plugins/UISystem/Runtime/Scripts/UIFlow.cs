using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UISystem
{
    public sealed class UIFlow
    {
        private readonly Dictionary<string, HashSet<string>> _groups = new(StringComparer.Ordinal);
        private readonly UIManager _ui;

        internal UIFlow(UIManager ui) => _ui = ui;

        #region  Windows

        public void Open<TVindowType, TData>(TData data)
            where TVindowType : struct, IWindowType
            => _ui.OpenController(GetId<TVindowType>(), data);

        public void Open<TVindowType>()
            where TVindowType : struct, IWindowType
            => _ui.OpenController(GetId<TVindowType>());

        public void Close<TVindowType>()
            where TVindowType : struct, IWindowType
            => _ui.CloseController(GetId<TVindowType>());

        public void Destroy<TVindowType>()
            where TVindowType : struct, IWindowType
            => _ui.Destroy(GetId<TVindowType>());

        public void Open<TData>(string windowId, TData data)
            => _ui.OpenView(windowId, data);

        public void Open(string windowId)
            => _ui.OpenController(windowId);

        public void Close(string windowId)
            => _ui.CloseController(windowId);


        internal void OpenView<TData>(string windowId, TData data)
            => _ui.OpenView(windowId, data);

        internal void OpenView(string windowId)
            => _ui.OpenView(windowId);

        internal void CloseView(string windowId)
            => _ui.CloseView(windowId);

        #endregion

        #region  Groups

        public void AddWindowToGroup<TGroupType, TWindowType>()
            where TWindowType : struct, IWindowType
            where TGroupType : struct, IGroupType
            => AddWindowToGroup(GetId<TGroupType>(), GetId<TWindowType>());

        public void CloseGroup<TGroupType>()
            where TGroupType : struct, IGroupType
            => CloseGroup(GetId<TGroupType>());

        public void AddWindowToGroup(string groupId, string windowId)
        {
            if (string.IsNullOrWhiteSpace(groupId)
                || string.IsNullOrWhiteSpace(windowId))
                return;

            GetOrCreateGroup(groupId).Add(windowId);
        }

        public void CloseGroup(string groupId)
        {
            if (!_groups.TryGetValue(groupId, out var viewIds)
                || viewIds == null
                || viewIds.Count == 0)
                return;

            foreach (var id in viewIds)
            {
                _ui.CloseController(id);
            }
        }

        private HashSet<string> GetOrCreateGroup(string groupId)
        {
            if (!_groups.TryGetValue(groupId, out var set) || set == null)
            {
                set = new HashSet<string>(StringComparer.Ordinal);
                _groups[groupId] = set;
            }

            return set;
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetId<T>()
            where T : struct, IHasId
        {
            var id = default(T).Id;
            if (string.IsNullOrWhiteSpace(id))
            {
                UnityEngine.Debug.LogWarning($"[UISystem] '{typeof(T).Name}' ID isn't set.");
                return typeof(T).Name;
            }

            return id;
        }
    }
}

