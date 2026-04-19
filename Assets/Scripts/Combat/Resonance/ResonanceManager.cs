using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    public enum ResonanceGainType
    {
        TakeHit,
        DealDamage,
        Support
    }

    // 場景 scoped，不 DontDestroyOnLoad
    public class ResonanceManager : MonoBehaviour
    {
        public static ResonanceManager Instance { get; private set; }

        private Dictionary<string, ResonanceData> resonanceMap = new Dictionary<string, ResonanceData>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_COMBAT_END, OnCombatEnd);
            EventManager.Instance.Subscribe(GameEvents.ON_MERCENARY_DIED, OnMercenaryDied);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_COMBAT_END, OnCombatEnd);
            EventManager.Instance.Unsubscribe(GameEvents.ON_MERCENARY_DIED, OnMercenaryDied);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void OnCombatEnd(EventData data)
        {
            // 結算崩鳴後座力，接口預留
            foreach (var pair in resonanceMap)
            {
                if (pair.Value.IsResonating)
                {
                    var endData = new EventData();
                    endData.Set("unitId", pair.Key);
                    EventManager.Instance.Publish(GameEvents.ON_RESONANCE_ENDED, endData);
                    // 後座力效果佔位：各角色定義後填入
                }
            }
            resonanceMap.Clear();
        }

        private void OnMercenaryDied(EventData data)
        {
            string unitId = data.Get<string>("unitId");
            if (!string.IsNullOrEmpty(unitId))
                resonanceMap.Remove(unitId);
        }

        public ResonanceData RegisterUnit(string unitId)
        {
            if (!resonanceMap.ContainsKey(unitId))
                resonanceMap[unitId] = new ResonanceData(unitId);
            return resonanceMap[unitId];
        }

        public void AddResonance(string unitId, ResonanceGainType gainType)
        {
            if (!resonanceMap.TryGetValue(unitId, out var rData)) return;

            float gain = gainType switch
            {
                ResonanceGainType.TakeHit    => ResonanceConfig.RESONANCE_GAIN_HIT,
                ResonanceGainType.DealDamage => ResonanceConfig.RESONANCE_GAIN_DEAL,
                ResonanceGainType.Support    => ResonanceConfig.RESONANCE_GAIN_SUPPORT,
                _                            => 0f
            };

            bool wasResonating = rData.IsResonating;
            rData.AddValue(gain);

            var chargeData = new EventData();
            chargeData.Set("unitId", unitId);
            chargeData.Set("value", rData.currentValue);
            EventManager.Instance.Publish(GameEvents.ON_RESONANCE_CHARGED, chargeData);

            if (!wasResonating && rData.IsResonating)
            {
                var trigData = new EventData();
                trigData.Set("unitId", unitId);
                EventManager.Instance.Publish(GameEvents.ON_RESONANCE_TRIGGERED, trigData);
            }
        }

        public void ConsumeResonancePoints(string unitId, int amount)
        {
            if (!resonanceMap.TryGetValue(unitId, out var rData)) return;
            bool wasResonating = rData.IsResonating;
            rData.ConsumePoints(amount);
            if (wasResonating && !rData.IsResonating)
            {
                var endData = new EventData();
                endData.Set("unitId", unitId);
                EventManager.Instance.Publish(GameEvents.ON_RESONANCE_ENDED, endData);
            }
        }

        public ResonanceData GetData(string unitId)
        {
            resonanceMap.TryGetValue(unitId, out var data);
            return data;
        }

        public bool BothResonating(string unitIdA, string unitIdB)
        {
            var a = GetData(unitIdA);
            var b = GetData(unitIdB);
            return a != null && b != null && a.IsResonating && b.IsResonating
                   && a.resonancePoints > 0 && b.resonancePoints > 0;
        }
    }
}
