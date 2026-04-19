using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    public class BattleStats
    {
        public float attack;
        public float defense;
        public List<string> unlockedAbilityIds = new List<string>();
        public List<object> moduleEffects = new List<object>();
    }

    public class BattleStatsBuilder : MonoBehaviour
    {
        private bool isLocked = false;
        private Dictionary<string, BattleStats> builtStats = new Dictionary<string, BattleStats>();

        // 供測試讀取鎖定狀態（唯讀）
        public bool IsLocked => isLocked;

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_COMBAT_START, OnCombatStart);
            EventManager.Instance.Subscribe(GameEvents.ON_COMBAT_END, OnCombatEnd);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_COMBAT_START, OnCombatStart);
            EventManager.Instance.Unsubscribe(GameEvents.ON_COMBAT_END, OnCombatEnd);
        }

        private void OnCombatStart(EventData data) => Build();

        private void OnCombatEnd(EventData data)
        {
            isLocked = false;
            builtStats.Clear();
        }

        public void Build()
        {
            if (isLocked)
            {
                Debug.LogWarning("[BattleStatsBuilder] Build() 被呼叫時 isLocked=true，拒絕重算。");
                return;
            }

            builtStats.Clear();

            var perks       = GrowthManager.Instance?.GetActivePerks()          ?? new List<PerkData>();
            var abilities   = GrowthManager.Instance?.GetUnlockedAbilities("Draven") ?? new List<AffinityUnlockData>();
            var moduleEffects = CoreModuleManager.Instance?.GetActiveModuleEffects() ?? new List<object>();

            builtStats["Draven"] = AssembleStats(perks, abilities, moduleEffects);

            isLocked = true;
            EventManager.Instance.Publish(GameEvents.ON_BATTLE_STATS_UPDATED, new EventData());
        }

        /// <summary>
        /// 三來源加總。可直接呼叫以注入測試資料，不依賴任何 Singleton。
        /// 加護技能 effectValue → attack；好感度解鎖 statBonus → attack；
        /// 魔核：Green → defense，Red/Blue → attack（佔位，待設計端定義後調整）。
        /// </summary>
        public BattleStats AssembleStats(
            List<PerkData> perks,
            List<AffinityUnlockData> abilities,
            List<object> moduleEffects)
        {
            var stats = new BattleStats();

            // 來源 1：加護技能
            foreach (var p in perks)
                stats.attack += p.effectValue;

            // 來源 2：好感度解鎖
            foreach (var ab in abilities)
                stats.attack += ab.statBonus;

            // 來源 3：魔核效果
            foreach (var raw in moduleEffects)
            {
                if (raw is not CoreModuleData mod || !mod.IsActive) continue;
                switch (mod.color)
                {
                    case ModuleColor.Green: stats.defense += mod.effectValue; break;
                    case ModuleColor.Red:
                    case ModuleColor.Blue:  stats.attack  += mod.effectValue; break;
                }
                stats.moduleEffects.Add(mod);
            }

            return stats;
        }

        public BattleStats GetStats(string unitId)
        {
            builtStats.TryGetValue(unitId, out var stats);
            return stats ?? new BattleStats();
        }
    }
}
