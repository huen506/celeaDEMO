using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    [System.Serializable]
    public class ActiveHiddenQuest
    {
        public string questId;
        public string characterId;
        public int    acceptedDay;
        public int    timeLimitDays;
        public int    successAffinityDelta;
        public int    failureAffinityDelta;
    }

    [System.Serializable]
    public class HiddenQuestSaveData
    {
        public List<ActiveHiddenQuest>   activeQuests  = new List<ActiveHiddenQuest>();
        public List<string>              usedQuestIds  = new List<string>();
    }

    public class HiddenQuestManager : MonoBehaviour
    {
        private const float HIDDEN_QUEST_TRIGGER_CHANCE = 0.3f;

        [Header("元件（在 Inspector 指定）")]
        [SerializeField] private FlagManager _flagManager;
        [SerializeField] private TimeManager _timeManager;

        private HiddenQuestDatabase             _database;
        private Dictionary<string, HiddenQuestPool> _pools = new Dictionary<string, HiddenQuestPool>();
        private readonly List<ActiveHiddenQuest>    _activeQuests = new List<ActiveHiddenQuest>();

        private void Awake()
        {
            LoadDatabase();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_CHARACTER_INVITED, OnCharacterInvited);
            EventManager.Instance.Subscribe(GameEvents.ON_DAY_END,           OnDayEnd);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_CHARACTER_INVITED, OnCharacterInvited);
            EventManager.Instance.Unsubscribe(GameEvents.ON_DAY_END,           OnDayEnd);
        }

        // ── 資料載入 ─────────────────────────────────────────────

        private void LoadDatabase()
        {
            var dbAsset = Resources.Load<TextAsset>("HiddenQuests/HiddenQuestDatabase");
            if (dbAsset == null)
            {
                Debug.LogWarning("[HiddenQuestManager] 找不到 HiddenQuestDatabase.json。");
                return;
            }
            _database = JsonUtility.FromJson<HiddenQuestDatabase>(dbAsset.text);
        }

        private HiddenQuestPool LoadPool(string characterId)
        {
            if (_pools.ContainsKey(characterId)) return _pools[characterId];
            var asset = Resources.Load<TextAsset>($"HiddenQuests/{characterId}_pool");
            if (asset == null) return null;
            var pool = JsonUtility.FromJson<HiddenQuestPool>(asset.text);
            _pools[characterId] = pool;
            return pool;
        }

        // ── 事件接收 ─────────────────────────────────────────────

        private void OnCharacterInvited(EventData data)
        {
            string characterId = data.Get<string>("characterId");
            if (string.IsNullOrEmpty(characterId)) return;

            if (Random.value >= HIDDEN_QUEST_TRIGGER_CHANCE) return;

            TryTriggerQuest(characterId);
        }

        private void OnDayEnd(EventData data)
        {
            int currentDay = _timeManager != null ? _timeManager.DayCount : 0;
            var failed = new List<ActiveHiddenQuest>();

            foreach (var q in _activeQuests)
            {
                if (q.timeLimitDays < 0) continue;
                if (currentDay - q.acceptedDay > q.timeLimitDays)
                    failed.Add(q);
            }

            foreach (var q in failed)
                FailQuest(q);
        }

        // ── 觸發邏輯 ─────────────────────────────────────────────

        private void TryTriggerQuest(string characterId)
        {
            if (_flagManager == null || _database == null) return;

            int tier = _flagManager.GetAffinityTier(characterId);
            var pool = LoadPool(characterId);
            if (pool == null) return;

            var tierList = pool.questsByTier?.Find(t => t.tier == tier);
            if (tierList == null || tierList.questIds == null || tierList.questIds.Count == 0) return;

            var candidates = new List<string>();
            foreach (string qId in tierList.questIds)
            {
                var qData = FindQuestData(qId);
                if (qData != null && !qData.isUsed)
                    candidates.Add(qId);
            }

            if (candidates.Count == 0) return;

            string chosen = candidates[Random.Range(0, candidates.Count)];
            var quest = FindQuestData(chosen);
            if (quest == null) return;

            quest.isUsed = true;

            int acceptedDay = _timeManager != null ? _timeManager.DayCount : 0;
            _activeQuests.Add(new ActiveHiddenQuest
            {
                questId             = quest.questId,
                characterId         = characterId,
                acceptedDay         = acceptedDay,
                timeLimitDays       = quest.timeLimitDays,
                successAffinityDelta = quest.successAffinityDelta,
                failureAffinityDelta = quest.failureAffinityDelta,
            });

            var triggerData = new EventData();
            triggerData.Set("characterId",      characterId);
            triggerData.Set("triggerDialogueId", quest.triggerDialogueId);
            EventManager.Instance.Publish(GameEvents.ON_HIDDEN_QUEST_TRIGGERED, triggerData);
        }

        private HiddenQuestData FindQuestData(string questId)
        {
            if (_database?.quests == null) return null;
            foreach (var q in _database.quests)
                if (q.questId == questId) return q;
            return null;
        }

        // ── 完成 / 失敗 ──────────────────────────────────────────

        public void CompleteQuest(string questId)
        {
            var active = _activeQuests.Find(q => q.questId == questId);
            if (active == null) return;

            _activeQuests.Remove(active);
            if (_flagManager != null)
                _flagManager.ModifyAffinity(active.characterId, active.successAffinityDelta);

            var completeData = new EventData();
            completeData.Set("questId",     questId);
            completeData.Set("characterId", active.characterId);
            EventManager.Instance.Publish(GameEvents.ON_HIDDEN_QUEST_COMPLETED, completeData);
        }

        private void FailQuest(ActiveHiddenQuest active)
        {
            _activeQuests.Remove(active);
            if (_flagManager != null)
                _flagManager.ModifyAffinity(active.characterId, active.failureAffinityDelta);

            var failData = new EventData();
            failData.Set("questId",     active.questId);
            failData.Set("characterId", active.characterId);
            EventManager.Instance.Publish(GameEvents.ON_HIDDEN_QUEST_FAILED, failData);
        }

        // ── 對外接口 ─────────────────────────────────────────────

        /// <summary>僅供測試用。繞過機率直接進入抽取邏輯，正式流程不使用。</summary>
        public void ForceTrigger(string characterId) => TryTriggerQuest(characterId);

        public List<ActiveHiddenQuest> GetActiveQuests() => new List<ActiveHiddenQuest>(_activeQuests);

        public HiddenQuestSaveData CaptureState()
        {
            var save = new HiddenQuestSaveData
            {
                activeQuests = new List<ActiveHiddenQuest>(_activeQuests),
            };
            if (_database?.quests != null)
                foreach (var q in _database.quests)
                    if (q.isUsed) save.usedQuestIds.Add(q.questId);
            return save;
        }

        public void RestoreState(HiddenQuestSaveData saveData)
        {
            if (saveData == null) return;
            _activeQuests.Clear();
            if (saveData.activeQuests != null)
                _activeQuests.AddRange(saveData.activeQuests);

            if (_database?.quests != null && saveData.usedQuestIds != null)
                foreach (var q in _database.quests)
                    q.isUsed = saveData.usedQuestIds.Contains(q.questId);
        }
    }
}
