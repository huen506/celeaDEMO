using System.Collections.Generic;

namespace Celea
{
    /// <summary>
    /// 事件攜帶的資料格式。
    /// 使用 key-value 字典傳遞任意參數，保持彈性。
    /// </summary>
    public class EventData
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public void Set(string key, object value)
        {
            _data[key] = value;
        }

        public T Get<T>(string key)
        {
            if (_data.TryGetValue(key, out object value) && value is T typedValue)
                return typedValue;
            return default;
        }

        public bool Has(string key)
        {
            return _data.ContainsKey(key);
        }
    }
}
