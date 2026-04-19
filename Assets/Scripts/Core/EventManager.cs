using System;
using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 全域事件管理系統。
    /// Singleton + DontDestroyOnLoad，整個遊戲只存在一個實例。
    /// 任何系統透過 EventManager.Instance 存取。
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        public static bool ENABLE_EVENT_LOG = true;

        private static EventManager _instance;

        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("EventManager");
                    _instance = go.AddComponent<EventManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private readonly Dictionary<string, List<Action<EventData>>> _listeners =
            new Dictionary<string, List<Action<EventData>>>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 訂閱事件。系統初始化時呼叫，記得在 OnDestroy 時呼叫 Unsubscribe。
        /// </summary>
        public void Subscribe(string eventName, Action<EventData> callback)
        {
            if (!_listeners.ContainsKey(eventName))
                _listeners[eventName] = new List<Action<EventData>>();

            _listeners[eventName].Add(callback);
        }

        /// <summary>
        /// 取消訂閱事件。系統銷毀時必須呼叫，避免記憶體洩漏。
        /// </summary>
        public void Unsubscribe(string eventName, Action<EventData> callback)
        {
            if (_listeners.TryGetValue(eventName, out List<Action<EventData>> callbacks))
                callbacks.Remove(callback);
        }

        /// <summary>
        /// 發出事件，廣播給所有登記的監聽方。
        /// 若無人監聽，照常發出，不報錯，自然消失。
        /// </summary>
        public void Publish(string eventName, EventData data = null)
        {
            if (!_listeners.TryGetValue(eventName, out List<Action<EventData>> callbacks))
                return;

            if (ENABLE_EVENT_LOG)
                Debug.Log($"[Event] {eventName}");

            // 複製一份，防止回調中途修改訂閱清單導致迭代錯誤
            List<Action<EventData>> snapshot = new List<Action<EventData>>(callbacks);
            EventData payload = data ?? new EventData();

            foreach (Action<EventData> callback in snapshot)
            {
                try
                {
                    callback?.Invoke(payload);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EventManager] 事件 '{eventName}' 的回調發生例外：{e}");
                }
            }
        }
    }
}
