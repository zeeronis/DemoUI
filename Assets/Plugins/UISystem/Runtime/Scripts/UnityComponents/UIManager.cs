using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UISystem
{
    public sealed class RuntimeWindowContext
    {
        public View View;
        public Controller Controller;
        public Channel Channel;
        public bool IsOpen;
    }

    [System.Serializable]
    public struct WindowConfig
    {
        public string WindowId;
        public string GroupId;
        public int Order;
        public View ViewPrefab;
    }

    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UIManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        [Tooltip("If disabled, call Initialize() yourself (e.g. from bootstrap).")]
        private bool _initializeOnAwake = true;
        [SerializeField]
        private Transform _windowsParent;

        [SerializeField]
        [Header("Window Configs")]
        private List<WindowConfig> _viewConfigs = new();

        private readonly Dictionary<string, RuntimeWindowContext> _existsWindowsById = new(System.StringComparer.Ordinal);
        private readonly HashSet<string> _activeWindowsIds = new(System.StringComparer.Ordinal);
        private readonly List<View> _iterableViewsBuffer = new();

        private WindowsComposer _windowComposer = new();
        private IUIFactory _factory;
        private bool _initialized;

        public UIFlow Flow { get; private set; }

        public IUIFactory Factory
        {
            get => _factory ??= new DefaultUIFactory();
            set => _factory = value ?? new DefaultUIFactory();
        }

        public void Initialize()
        {
            if (_initialized)
                return;

            _windowComposer.MapWindowConfigs(_viewConfigs);
            Flow = new UIFlow(this);
            _initialized = true;
        }

        public void BindControllerToWindow<TController>(string windowId)
            where TController : Controller
        {
            if (string.IsNullOrWhiteSpace(windowId))
            {
                Debug.LogError("[UISystem] Window id is empty.");
                return;
            }

            _windowComposer.Bind<TController>(windowId);
        }

        public void BindControllerToWindow<TController, TWindowType>()
            where TWindowType : struct, IWindowType
            where TController : Controller
        {
            BindControllerToWindow<TController>(default(TWindowType).Id);
        }

        private void Awake()
        {
            if (_initializeOnAwake)
                Initialize();
        }

        private void Update()
        {
            if (!IsInitialized(errorLog: false))
                return;

            foreach (var item in GetActiveWindows())
                item.TickUpdate();
        }

        private void LateUpdate()
        {
            if (!IsInitialized(errorLog: false))
                return;

            foreach (var item in GetActiveWindows())
                item.TickLateUpdate();
        }

        private void FixedUpdate()
        {
            if (!IsInitialized(errorLog: false))
                return;

            foreach (var item in GetActiveWindows())
                item.TickFixedUpdate();
        }

        private bool IsInitialized(bool errorLog = true)
        {
            if (_initialized)
                return true;

            if (errorLog)
                Debug.LogError("[UISystem] UIManager is not initialized. Enable 'Initialize On Awake' or call Initialize() manually.");

            return false;
        }

        internal void OpenView(string windowId)
        {
            if (!TryGetOrCreateWindow(windowId, out var windowContext)
                || windowContext.IsOpen)
                return;

            _activeWindowsIds.Add(windowId);
            windowContext.IsOpen = true;
            windowContext.View.Open();
        }

        internal void OpenView<TData>(string windowId, TData data)
        {
            if (!TryGetOrCreateWindow(windowId, out var windowContext)
                || windowContext.IsOpen)
                return;

            _activeWindowsIds.Add(windowId);
            windowContext.IsOpen = true;
            windowContext.Channel.SendState(data);
            windowContext.View.Open();
        }

        internal void OpenController(string windowId)
        {
            if (!TryGetOrCreateWindow(windowId, out var windowContext)
                || windowContext.IsOpen)
                return;

            windowContext.Controller.OnOpenWindowRequest();
        }

        internal void OpenController<TData>(string windowId, TData data)
        {
            if (!TryGetOrCreateWindow(windowId, out var windowContext)
                || windowContext.IsOpen)
                return;

            windowContext.Channel.SendIntent(data);
            windowContext.Controller.OnOpenWindowRequest();
        }

        internal void CloseView(string windowId)
        {
            if (!TryGetWindow(windowId, out var windowContext)
                || !windowContext.IsOpen)
                return;

            _activeWindowsIds.Remove(windowId);
            windowContext.IsOpen = false;
            windowContext.View.Close();
        }

        internal void CloseController(string windowId)
        {
            if (!TryGetWindow(windowId, out var windowContext)
                || !windowContext.IsOpen)
                return;

            windowContext.Controller.OnCloseWindowRequest();
        }

        internal void Destroy(string windowId)
        {
            if (!TryGetWindow(windowId, out var windowContext))
                return;

            if (windowContext?.View != null)
            {
                windowContext.View.Destroy();
                Destroy(windowContext.View.gameObject);
            }

            _activeWindowsIds.Remove(windowId);
            _existsWindowsById.Remove(windowId);
            return;
        }

        internal void SendDataToView<TData>(string windowId, TData data)
        {
            if (!TryGetWindow(windowId, out var windowContext)
                || windowContext?.Channel == null)
                return;

            windowContext.Channel.SendState(data);
        }

        internal void SendDataToController<TData>(string windowId, TData data)
        {
            if (!TryGetWindow(windowId, out var windowContext)
                || windowContext?.Channel == null)
                return;

            windowContext.Channel.SendIntent(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetWindow(string windowId, out RuntimeWindowContext windowContext)
        {
            if (!IsInitialized() || string.IsNullOrWhiteSpace(windowId))
            {
                windowContext = default;
                return false;
            }

            return TryGetWindowNoCheck(windowId, out windowContext);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetWindowNoCheck(string windowId, out RuntimeWindowContext windowContext)
        {
            return _existsWindowsById.TryGetValue(windowId, out windowContext);
        }

        private bool TryGetOrCreateWindow(string windowId, out RuntimeWindowContext windowContext)
        {
            if (!IsInitialized() || string.IsNullOrWhiteSpace(windowId))
            {
                windowContext = default;
                return false;
            }

            if (TryGetWindowNoCheck(windowId, out windowContext))
                return true;

            var parent = _windowsParent != null ? _windowsParent : transform;
            windowContext = _windowComposer.CreateWindow(windowId, Factory, parent, Flow);
            if (IsWindowContextValid(windowContext))
            {
                _existsWindowsById[windowId] = windowContext;
                windowContext.Controller?.Created();
                windowContext.View.Created();
                return true;
            }

            return false;
        }

        private bool IsWindowContextValid(RuntimeWindowContext context)
        {
            return context != null
                && context.View != null
                && context.Channel != null;
        }
        
        private List<View> GetActiveWindows()
        {
            _iterableViewsBuffer.Clear();
            foreach (var item in _activeWindowsIds)
            {
                if (!TryGetWindow(item, out var windowContext)
                    || windowContext.View == null)
                    continue;

                _iterableViewsBuffer.Add(windowContext.View);
            }

            return _iterableViewsBuffer;
        }
    }
}
