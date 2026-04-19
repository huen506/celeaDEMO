using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 局勢旗標。累積傾向數值，章末結算線路判斷。
    /// 各章節判斷條件由敘事端定義後注入，不硬編碼。
    /// </summary>
    public class SituationFlags
    {
        // flagId → 累積數值
        private Dictionary<string, int> _values = new Dictionary<string, int>();

        // ── 三種觸發接口（分類清單待敘事端提供後填入）────────────

        /// <summary>世界自發觸發：外部環境事件。</summary>
        public void TriggerWorldEvent(string flagId, int delta)
        {
            ApplyDelta(flagId, delta, "World");
        }

        /// <summary>德拉文觸發：夥伴行動。</summary>
        public void TriggerDravenEvent(string flagId, int delta)
        {
            ApplyDelta(flagId, delta, "Draven");
        }

        /// <summary>玩家主動觸發：玩家行動。</summary>
        public void TriggerPlayerEvent(string flagId, int delta)
        {
            ApplyDelta(flagId, delta, "Player");
        }

        /// <summary>回傳指定局勢旗標的累積數值。</summary>
        public int GetValue(string flagId)
        {
            _values.TryGetValue(flagId, out int val);
            return val;
        }

        /// <summary>
        /// 章末結算。敘事端注入判斷條件 chapterConditions 後呼叫。
        /// 條件格式待敘事端提供後填入，此處預留接口。
        /// </summary>
        public void EvaluateChapterEnd()
        {
            // 待敘事端定義判斷條件後實作
            Debug.Log("[SituationFlags] 章末結算：待敘事端定義判斷條件後填入邏輯。");
        }

        /// <summary>打包存檔資料。</summary>
        public SituationFlagsSaveData CaptureState()
        {
            var save = new SituationFlagsSaveData();
            save.keys   = new List<string>(_values.Keys);
            save.values = new List<int>(_values.Values);
            return save;
        }

        /// <summary>從存檔資料還原。</summary>
        public void RestoreState(SituationFlagsSaveData saveData)
        {
            _values.Clear();
            if (saveData.keys == null) return;
            for (int i = 0; i < saveData.keys.Count; i++)
                _values[saveData.keys[i]] = saveData.values[i];
        }

        private void ApplyDelta(string flagId, int delta, string source)
        {
            if (!_values.ContainsKey(flagId)) _values[flagId] = 0;
            _values[flagId] += delta;
            Debug.Log($"[SituationFlags] [{source}] {flagId} += {delta} → {_values[flagId]}");
        }
    }

    [System.Serializable]
    public class SituationFlagsSaveData
    {
        public List<string> keys;
        public List<int> values;
    }
}
