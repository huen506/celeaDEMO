using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    // 全域持久，Singleton + DontDestroyOnLoad
    public class InjuryManager : MonoBehaviour
    {
        public static InjuryManager Instance { get; private set; }

        private Dictionary<string, InjuryData> injuryMap = new Dictionary<string, InjuryData>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_UNIT_DEFEATED,   OnUnitDefeated);
            EventManager.Instance.Subscribe(GameEvents.ON_MERCENARY_DIED,  OnMercenaryDied);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_UNIT_DEFEATED,  OnUnitDefeated);
            EventManager.Instance.Unsubscribe(GameEvents.ON_MERCENARY_DIED, OnMercenaryDied);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void OnUnitDefeated(EventData data)
        {
            string unitId = data.Get<string>("unitId");
            if (string.IsNullOrEmpty(unitId)) return;
            if (unitId == "Draven") return; // 德拉文不走三階段，由 CombatManager 直接 Game Over
            ApplyInjury(unitId);
        }

        private void OnMercenaryDied(EventData data)
        {
            string unitId = data.Get<string>("unitId");
            if (string.IsNullOrEmpty(unitId)) return;

            if (injuryMap.TryGetValue(unitId, out var iData))
            {
                iData.SetState(InjuryState.Terminated);
                injuryMap.Remove(unitId);
            }
        }

        // 唯一可修改傷勢狀態的入口，外部禁止直接賦值
        public void ApplyInjury(string unitId)
        {
            var iData = GetOrCreate(unitId);
            iData.IncrementKnockdown();

            var newState = iData.knockdownCount switch
            {
                1 => InjuryState.Injured,
                2 => InjuryState.Critical,
                _ => InjuryState.Dead
            };
            iData.SetState(newState);

            // ON_INJURY_CHANGED 先發：此時 state = Injured / Critical / Dead
            // 必須在 ON_MERCENARY_DIED 之前發，否則 OnMercenaryDied 會將 state 改為 Terminated
            var injData = new EventData();
            injData.Set("unitId", unitId);
            injData.Set("state", iData.state.ToString());
            EventManager.Instance.Publish(GameEvents.ON_INJURY_CHANGED, injData);

            // Dead 狀態後再發 ON_MERCENARY_DIED → 觸發 OnMercenaryDied → Terminated + 從 map 移除
            if (iData.state == InjuryState.Dead)
            {
                var deadData = new EventData();
                deadData.Set("unitId", unitId);
                EventManager.Instance.Publish(GameEvents.ON_MERCENARY_DIED, deadData);
            }
        }

        public InjuryData GetInjuryData(string unitId) => GetOrCreate(unitId);

        private InjuryData GetOrCreate(string unitId)
        {
            if (!injuryMap.TryGetValue(unitId, out var data))
            {
                data = new InjuryData(unitId);
                injuryMap[unitId] = data;
            }
            return data;
        }

        // SaveSystem 接口
        public InjurySaveData CaptureState()
        {
            var save = new InjurySaveData();
            foreach (var kvp in injuryMap) save.entries.Add(kvp.Value);
            return save;
        }

        public void RestoreState(InjurySaveData saved)
        {
            injuryMap.Clear();
            if (saved == null) return;
            foreach (var entry in saved.entries)
                injuryMap[entry.unitId] = entry;
        }
    }
}
