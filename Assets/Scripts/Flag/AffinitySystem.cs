using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 好感度系統。各角色獨立累積，上下限截斷，階段判斷。
    /// Demo 範圍：正數階段完整實作；負數階段標記為待後續擴充。
    /// </summary>
    public class AffinitySystem
    {
        // 好感度數值上下限
        private const int MAX_AFFINITY =  3;
        private const int MIN_AFFINITY = -3;

        // 好感度數值 → 階段（-3 到 +3，共七階）
        // 各階段解鎖能力待設計端定義後填入，預留接口
        public const int TIER_COLD_MAX     = -3;
        public const int TIER_COLD         = -2;
        public const int TIER_DISTANT      = -1;
        public const int TIER_NEUTRAL      =  0;
        public const int TIER_WARM         =  1;
        public const int TIER_CLOSE        =  2;
        public const int TIER_BONDED       =  3;

        // characterId → 好感度數值
        private Dictionary<string, int> _values = new Dictionary<string, int>();

        /// <summary>調整指定角色的好感度，超出範圍截斷。</summary>
        public void AdjustAffinity(string characterId, int delta)
        {
            if (!_values.ContainsKey(characterId)) _values[characterId] = 0;
            int newValue = Mathf.Clamp(_values[characterId] + delta, MIN_AFFINITY, MAX_AFFINITY);
            _values[characterId] = newValue;
            Debug.Log($"[AffinitySystem] {characterId} += {delta} → {newValue}，階段 {GetTier(characterId)}");
        }

        /// <summary>回傳指定角色的好感度數值（-3 到 +3）。</summary>
        public int GetValue(string characterId)
        {
            _values.TryGetValue(characterId, out int val);
            return val;
        }

        /// <summary>回傳指定角色的好感度階段（-3 到 +3）。</summary>
        public int GetTier(string characterId)
        {
            return GetValue(characterId); // 數值即階段，範圍已截斷
        }

        /// <summary>打包存檔資料。</summary>
        public AffinitySaveData CaptureState()
        {
            var save = new AffinitySaveData();
            save.characterIds = new List<string>(_values.Keys);
            save.values       = new List<int>(_values.Values);
            return save;
        }

        /// <summary>從存檔資料還原。</summary>
        public void RestoreState(AffinitySaveData saveData)
        {
            _values.Clear();
            if (saveData.characterIds == null) return;
            for (int i = 0; i < saveData.characterIds.Count; i++)
                _values[saveData.characterIds[i]] = saveData.values[i];
        }
    }

    [System.Serializable]
    public class AffinitySaveData
    {
        public List<string> characterIds;
        public List<int> values;
    }
}
