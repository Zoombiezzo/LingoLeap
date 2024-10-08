using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.WindowsSystem.Scripts
{
    public sealed class WindowsService
    {
        internal const int MaxOrder = 32767;
        
        private static WindowsService s_instance = null;

        private Dictionary<string, Window> m_windows = new Dictionary<string, Window>();
        private Dictionary<Type, Window> m_windowTypes = new Dictionary<Type, Window>();

        private LinkedList<Window> m_openWindows = new LinkedList<Window>();
        
        private Dictionary<string, int> m_openWindowIDs = new Dictionary<string, int>();
        private HashSet<int> m_orderedWindow = new HashSet<int>();

        private Action<Window> _onShowedWindow;
        private Action<Window> _onHidedWindow;
        
        public static Action<Window> OnShowedWindow
        {
            get => s_instance._onShowedWindow;
            set => s_instance._onShowedWindow = value;
        }
        
        public static Action<Window> OnHidedWindow
        {
            get => s_instance._onHidedWindow;
            set => s_instance._onHidedWindow = value;
        }

        public static string GetId<T>() where T : Window
        {
            if (s_instance == null) return string.Empty;
            
            return s_instance.m_windowTypes.TryGetValue(typeof(T), out var window) == false ? string.Empty : window.Id;
        }

        public static void Show(string id)
        {
           s_instance?.ShowInternal(id);
        }
        
        public static void Show<T>() where T : Window => s_instance?.ShowInternal(GetId<T>());

        public static void Hide(string id)
        {
            s_instance?.HideInternal(id);
        }
        
        public static void Hide<T>() where T : Window => s_instance?.HideInternal(GetId<T>());
        
        public static bool IsOpen(string id)
        {
            if (s_instance == null) return false;
            
            return s_instance.IsOpenInternal(id);
        }

        public static bool IsOpen<T>() where T : Window => s_instance.IsOpenInternal(GetId<T>());

        public static bool IsOpenAny(params string[] ids)
        {
            if (s_instance == null) return false;

            foreach (var id in ids)
            {
                if (s_instance.IsOpenInternal(id)) return true;
            }

            return false;
        }
        
        public static bool IsCurrentState(params string[] ids)
        {
            if (s_instance == null) return false;
            
            if(s_instance.m_openWindowIDs.Count != ids.Length) return false;
            
            foreach (var id in ids)
            {
                if (s_instance.IsOpenInternal(id) == false) return false;
            }

            return true;
        }

        public static bool TryGetWindow(in string id, out Window window) => s_instance.TryGetRegisterWindow(id, out window);

        public static bool TryGetWindow<T>(in string id, out T window) where T : Window
        {
            window = null;

            var result = s_instance.TryGetRegisterWindow(id, out var windowRef);

            if (result == false)
                return false;

            if (windowRef is not T windowType) return false;
            
            window = windowType;
            return true;
        }

        public static bool TryGetWindow<T>(out T window) where T : Window
        {
            window = null;
            
            if (s_instance == null) return false;
            
            if (s_instance.m_windowTypes.TryGetValue(typeof(T), out var windowType) == false)
                return false;
            
            window = windowType as T;
            return true;
        }

        public static void Unregister(in Window window)
        {
            s_instance?.UnregisterInternal(window);
        }

        public static void Register(in Window window)
        {
            s_instance?.RegisterInternal(window);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            s_instance = new WindowsService();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RegisterInternal(in Window window)
        {
            if (window == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[WindowManager]: Register window failed! Window is null!");
#endif
                return;
            }

            if (m_windows.ContainsKey(window.Id))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[WindowManager]: Register window [{window.Id}] failed! Window is already registered!");
#endif
                return;
            }
            
            m_windows.Add(window.Id, window);
            
            var type = window.GetType();
            
            if (type != typeof(Window))
            {
                m_windowTypes.TryAdd(type, window);
            }

            window.Initialize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnregisterInternal(in Window window)
        {
            if (m_windows.ContainsKey(window.Id) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[WindowManager]: Unregister window [{window.Id}] failed! Window is not registered!");
#endif
                return;
            }
            
            m_windows.Remove(window.Id);
            
            var type = window.GetType();
            
            if (type != typeof(Window))
            {
                m_windowTypes.Remove(type);
            }

            if (m_openWindows.Contains(window))
            {
                RemoveOpenedWindow(window);
            }

#if UNITY_EDITOR
            Debug.LogWarning($"[WindowManager]: Window [{window.Id}] unregister!");
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsOpenInternal(string id)
        {
            return m_openWindowIDs.ContainsKey(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ShowInternal(in string id)
        {
            if (TryGetRegisterWindow(id, out var window) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[WindowManager]: Show window [{id}] failed! Window is not register!");
#endif
                return;
            }

            if (m_openWindows.Contains(window))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[WindowManager]: Show window [{id}] failed! Window is already showed!");
#endif
                return; 
            }

            if (window.IsShowing())
            {
#if UNITY_EDITOR
                Debug.Log("Window is showing");
#endif
                return;
            }
            
            ShowWindow(window);
#if UNITY_EDITOR
            Debug.Log($"Window was showed {id}");
#endif
            
            _onShowedWindow?.Invoke(window);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ShowWindow(Window window)
        {
            var sortingOrder = CalculateOrderWindow(window);
            var node = m_openWindows.AddLast(window);
            DisableRenderingPreviousWindows(node);

            window.ShowInternal(sortingOrder);

            if (m_openWindowIDs.ContainsKey(window.Id))
            {
                m_openWindowIDs[window.Id] += 1;
            }
            else
            {
                m_openWindowIDs.Add(window.Id, 1);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DisableRenderingPreviousWindows(LinkedListNode<Window> node)
        {
            if(node == null)
                return;
            
            var value = node.Value;
            
            if(value.DisablePreviousWindowsRendering == false)
                return;
            
            var previewNode = node.Previous;
            
            if(previewNode == null)
                return;
            
            var previewValue = previewNode.Value;

            while (previewValue != null)
            {
                if (previewValue.IgnoreDisableRendering == false)
                    previewValue.DisableRendering();

                if(previewValue.DisablePreviousWindowsRendering)
                    break;
                
                previewNode = previewNode.Previous;
                
                if(previewNode == null)
                    break;
                
                previewValue = previewNode.Value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnableRenderingPreviousWindows(LinkedListNode<Window> node)
        {
            if(node == null)
                return;
            
            var value = node.Value;
            
            if(value.DisablePreviousWindowsRendering == false)
                return;
            
            var previewNode = node.Previous;
            
            if(previewNode == null)
                return;
            
            var previewValue = previewNode.Value;

            while (previewValue != null)
            {
                if (previewValue.IgnoreDisableRendering == false)
                    previewValue.EnableRendering();

                if(previewValue.DisablePreviousWindowsRendering)
                    break;
                
                previewNode = previewNode.Previous;
                
                if(previewNode == null)
                    break;
                
                previewValue = previewNode.Value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CalculateOrderWindow(Window window)
        {
            if (window.IsStaticOrder)
            {
                return window.PreferredOrder;
            }

            int order;
            if (window.PreferredOrder > 0)
            {
                order = TryGetFreeOrder(window.PreferredOrder);
                TryAddOrderedWindow(order);
                return order;
            }

            order = TryGetFreeOrder(0);
            TryAddOrderedWindow(order);
            return order;
        }

        private void TryAddOrderedWindow(int order)
        {
            if (m_orderedWindow.Contains(order) == false)
            {
                m_orderedWindow.Add(order);
            }
        }

        private int TryGetFreeOrder(int order)
        {
            while (m_orderedWindow.Contains(order))
            {
                if (order >= MaxOrder)
                {
                    order = MaxOrder;
                    return order;
                }

                order++;
            }

            return order;
        }
        
        private void RecalculateOrdersOpenWindows()
        {
            m_orderedWindow.Clear();
            foreach (var window in m_openWindows)
            {
                var order = CalculateOrderWindow(window);
                if(window.SortingOrder == order) continue;
                
                window.SetSortingOrder(order);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HideInternal(in string id)
        {
            if (TryGetRegisterWindow(id, out var window) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[WindowManager]: Hide window [{id}] failed! Window is not register!");
#endif
                return;
            }
            
            if(m_openWindows.Contains(window) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[WindowManager]: Hide window [{id}] failed! Window is not showed!");
#endif
                return;
            }
            
            if (window.IsHiding())
            {
#if UNITY_EDITOR
                Debug.Log("Window is hiding");
#endif
                return;
            }

            HideWindow(window);
#if UNITY_EDITOR
            Debug.Log($"Window was hidden {id}");
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HideWindow(Window window)
        {
            var node = m_openWindows.Find(window);
            EnableRenderingPreviousWindows(node);
            
            window.HideInternal(() => RemoveOpenedWindow(window));
        }

        private void RemoveOpenedWindow(Window window)
        {
            m_openWindows.Remove(window);
            RecalculateOrdersOpenWindows();

            if (m_openWindowIDs.ContainsKey(window.Id))
            {
                m_openWindowIDs[window.Id] -= 1;
                if (m_openWindowIDs[window.Id] <= 0)
                {
                    m_openWindowIDs.Remove(window.Id);
                }
            }
            
            _onHidedWindow?.Invoke(window);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetRegisterWindow(in string id, out Window window)
        {
            window = null;
            if (m_windows.ContainsKey(id) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[WindowManager]: Failed get window with id: {id}!");
#endif
                return false;
            }

            window = m_windows[id];
            return true;
        }


    }
}