using UnityEngine;

namespace Celea
{
    public enum InjuryState
    {
        Normal,
        Injured,
        Critical,
        Dead,
        Terminated  // 傭兵永久死亡後清除用
    }

    // 傷勢的唯一資料來源，CombatUnit 只持有此類別的唯讀引用
    [System.Serializable]
    public class InjuryData
    {
        public string unitId;

        // SerializeField 讓 JsonUtility 仍可序列化，但外部無公開 setter
        [SerializeField] private InjuryState _state = InjuryState.Normal;
        [SerializeField] private int _knockdownCount = 0;

        public InjuryState state       => _state;
        public int         knockdownCount => _knockdownCount;

        // 佔位：各狀態自然恢復時間（天），待設計端定義後填入
        public const float INJURED_RECOVERY_DAYS  = 3f;
        public const float CRITICAL_RECOVERY_DAYS = 7f;

        // 佔位：傷勢 debuff 能力值下降幅度，待設計端定義
        public const float INJURED_STAT_PENALTY  = 0.15f;
        public const float CRITICAL_STAT_PENALTY = 0.30f;

        public bool IsAlive => _state != InjuryState.Dead && _state != InjuryState.Terminated;

        public InjuryData(string id) { unitId = id; }

        // InjuryManager 專屬入口，外部類別不應直接呼叫
        internal void IncrementKnockdown() { _knockdownCount++; }
        internal void SetState(InjuryState newState) { _state = newState; }
    }
}
