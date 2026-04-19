using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    [System.Serializable]
    public class GrowthSaveData
    {
        public List<string> acquiredPerkIds = new List<string>();
        public List<AffinityUnlockData> affinityUnlocks = new List<AffinityUnlockData>();
    }

    // 全域持久，Singleton + DontDestroyOnLoad
    public class GrowthManager : MonoBehaviour
    {
        public static GrowthManager Instance { get; private set; }

        private PerkDatabase perkDatabase = new PerkDatabase();
        private List<PerkData> acquiredPerks = new List<PerkData>();
        private Dictionary<string, List<AffinityUnlockData>> affinityUnlocks
            = new Dictionary<string, List<AffinityUnlockData>>();

        // 善惡光譜門檻（已確認 2026-04-18）
        private const float VIRTUE_THRESHOLD  =  25f;
        private const float SIN_THRESHOLD     = -25f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            perkDatabase.Load();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_CHAPTER_CLEARED, OnChapterCleared);
            EventManager.Instance.Subscribe(GameEvents.ON_AFFINITY_TIER_UNLOCKED, OnAffinityTierUnlocked);
            EventManager.Instance.Subscribe(GameEvents.ON_PERK_SELECTED, OnPerkSelected);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_CHAPTER_CLEARED, OnChapterCleared);
            EventManager.Instance.Unsubscribe(GameEvents.ON_AFFINITY_TIER_UNLOCKED, OnAffinityTierUnlocked);
            EventManager.Instance.Unsubscribe(GameEvents.ON_PERK_SELECTED, OnPerkSelected);
        }

        private void OnChapterCleared(EventData data)
        {
            var flagManager = UnityEngine.Object.FindAnyObjectByType<FlagManager>();
            float moralValue = flagManager != null ? flagManager.GetMoralValue() : 0f;
            PerkPoolType pool = moralValue >= VIRTUE_THRESHOLD ? PerkPoolType.Virtue
                              : moralValue <= SIN_THRESHOLD   ? PerkPoolType.Sin
                                                              : PerkPoolType.Neutral;

            var available = GetAvailablePerks(pool);
            if (available.Count == 0)
            {
                // 全部已獲得，給予固定能力值提升（佔位）
                Debug.Log("[GrowthManager] 技能池已全部獲得，發放固定能力值提升（佔位）。");
                return;
            }

            // 隨機抽三個（或全部）
            var options = DrawPerks(available, 3);
            var offerData = new EventData();
            offerData.Set("options", options);
            EventManager.Instance.Publish(GameEvents.ON_PERK_SELECTED, offerData);
            // 實際選擇由 UI 完成後再觸發 ON_PERK_SELECTED 帶 selectedPerkId
        }

        private void OnPerkSelected(EventData data)
        {
            string perkId = data.Get<string>("selectedPerkId");
            if (string.IsNullOrEmpty(perkId)) return;

            var perk = FindPerkById(perkId);
            if (perk != null && !acquiredPerks.Exists(p => p.perkId == perkId))
                acquiredPerks.Add(perk);
        }

        private void OnAffinityTierUnlocked(EventData data)
        {
            string characterId = data.Get<string>("characterId");
            int tier = data.Get<int>("tier");
            // 觸發對應劇情，隔天回城後播放（接口預留）
            Debug.Log($"[GrowthManager] {characterId} 好感度達到 Tier {tier}，待敘事端定義劇情觸發。");
        }

        // BattleStatsBuilder 查詢接口
        public List<PerkData> GetActivePerks() => new List<PerkData>(acquiredPerks);

        public List<AffinityUnlockData> GetUnlockedAbilities(string characterId)
        {
            affinityUnlocks.TryGetValue(characterId, out var list);
            return list ?? new List<AffinityUnlockData>();
        }

        private List<PerkData> GetAvailablePerks(PerkPoolType pool)
        {
            var poolList = perkDatabase.GetPool(pool);
            var result = new List<PerkData>();
            foreach (var p in poolList)
                if (!acquiredPerks.Exists(a => a.perkId == p.perkId))
                    result.Add(p);
            return result;
        }

        private List<PerkData> DrawPerks(List<PerkData> source, int count)
        {
            var copy = new List<PerkData>(source);
            var result = new List<PerkData>();
            for (int i = 0; i < count && copy.Count > 0; i++)
            {
                int idx = Random.Range(0, copy.Count);
                result.Add(copy[idx]);
                copy.RemoveAt(idx);
            }
            return result;
        }

        private PerkData FindPerkById(string id)
        {
            foreach (var pool in new[] { PerkPoolType.Virtue, PerkPoolType.Neutral, PerkPoolType.Sin })
                foreach (var p in perkDatabase.GetPool(pool))
                    if (p.perkId == id) return p;
            return null;
        }

        // SaveSystem 接口
        public GrowthSaveData CaptureState()
        {
            var save = new GrowthSaveData();
            foreach (var p in acquiredPerks) save.acquiredPerkIds.Add(p.perkId);
            foreach (var kvp in affinityUnlocks) save.affinityUnlocks.AddRange(kvp.Value);
            return save;
        }

        public void RestoreState(GrowthSaveData saved)
        {
            acquiredPerks.Clear();
            affinityUnlocks.Clear();
            foreach (var id in saved.acquiredPerkIds)
            {
                var p = FindPerkById(id);
                if (p != null) acquiredPerks.Add(p);
            }
            foreach (var unlock in saved.affinityUnlocks)
            {
                if (!affinityUnlocks.ContainsKey(unlock.characterId))
                    affinityUnlocks[unlock.characterId] = new List<AffinityUnlockData>();
                affinityUnlocks[unlock.characterId].Add(unlock);
            }
        }
    }
}
