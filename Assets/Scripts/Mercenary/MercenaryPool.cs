using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    [System.Serializable]
    public class MercenarySaveData
    {
        public List<MercenaryData> mercenaryStates = new List<MercenaryData>();
    }

    // 全域持久，Singleton + DontDestroyOnLoad
    public class MercenaryPool : MonoBehaviour
    {
        public static MercenaryPool Instance { get; private set; }

        // 佔位：拒絕機率，待設計端定義後填入
        private const float REFUSAL_CHANCE = 0.3f;

        private MercenaryDatabase database = new MercenaryDatabase();
        private List<MercenaryData> pool = new List<MercenaryData>();
        private List<MercenaryData> currentParty = new List<MercenaryData>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            database.Load();
            pool = database.GetAll();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_MERCENARY_DIED, OnMercenaryDied);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_MERCENARY_DIED, OnMercenaryDied);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void OnMercenaryDied(EventData data)
        {
            string unitId = data.Get<string>("unitId");
            var merc = pool.Find(m => m.mercenaryId == unitId);
            if (merc != null)
            {
                merc.isAlive = false;
                currentParty.RemoveAll(m => m.mercenaryId == unitId);
                EventManager.Instance.Publish(GameEvents.ON_MERCENARY_DISMISSED, data);
            }
        }

        // BattleStatsBuilder 查詢接口
        public List<MercenaryData> GetCurrentParty() => new List<MercenaryData>(currentParty);

        // 光譜篩選：回傳當前光譜下可招募的清單
        public List<MercenaryData> GetAvailableMercenaries()
        {
            var flagManager = UnityEngine.Object.FindAnyObjectByType<FlagManager>();
            int moralTier = flagManager != null ? flagManager.GetMoralTier() : 0;
            // tier int → string 對應：正=Virtue，0=Neutral，負=Sin
            string currentTier = moralTier > 0 ? "Virtue" : moralTier < 0 ? "Sin" : "Neutral";

            string currentRoute = "Tech"; // 佔位：從 FlagManager 查詢線路後填入

            var result = new List<MercenaryData>();
            foreach (var m in pool)
            {
                if (!m.isAlive) continue;
                if (!string.IsNullOrEmpty(m.routeExclusive) && m.routeExclusive != currentRoute) continue;
                if (m.preferredMoralTiers.Count > 0 && !m.preferredMoralTiers.Contains(currentTier)) continue;
                result.Add(m);
            }
            return result;
        }

        public bool TryRecruit(string mercenaryId)
        {
            if (currentParty.Count >= 2) return false;

            var merc = pool.Find(m => m.mercenaryId == mercenaryId && m.isAlive);
            if (merc == null) return false;

            // 拒絕機率
            if (Random.value < REFUSAL_CHANCE) return false;

            currentParty.Add(merc);
            var recruitData = new EventData();
            recruitData.Set("mercenaryId", mercenaryId);
            EventManager.Instance.Publish(GameEvents.ON_MERCENARY_RECRUITED, recruitData);
            return true;
        }

        public void Dismiss(string mercenaryId)
        {
            currentParty.RemoveAll(m => m.mercenaryId == mercenaryId);
            var data = new EventData();
            data.Set("mercenaryId", mercenaryId);
            EventManager.Instance.Publish(GameEvents.ON_MERCENARY_DISMISSED, data);
        }

        // SaveSystem 接口
        public MercenarySaveData CaptureState()
        {
            return new MercenarySaveData { mercenaryStates = new List<MercenaryData>(pool) };
        }

        public void RestoreState(MercenarySaveData saved)
        {
            pool = saved.mercenaryStates ?? new List<MercenaryData>();
        }
    }
}
