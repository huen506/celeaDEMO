using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    [System.Serializable]
    public class NoticeBoardSaveData
    {
        public List<NoticeBoardQuestData> quests = new List<NoticeBoardQuestData>();
    }

    public class NoticeBoardManager : MonoBehaviour
    {
        [Header("元件（在 Inspector 指定）")]
        [SerializeField] private TimeManager _timeManager;

        private readonly List<NoticeBoardQuestData> _allQuests = new List<NoticeBoardQuestData>();

        private void Awake()
        {
            LoadAllTownData();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_DAY_END, OnDayEnd);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_DAY_END, OnDayEnd);
        }

        // ── 資料載入 ─────────────────────────────────────────────

        private void LoadAllTownData()
        {
            var assets = Resources.LoadAll<TextAsset>("NoticeBoard");
            foreach (var asset in assets)
            {
                var collection = JsonUtility.FromJson<TownQuestCollection>(asset.text);
                if (collection?.quests == null) continue;
                _allQuests.AddRange(collection.quests);
            }
        }

        // ── 事件接收 ─────────────────────────────────────────────

        private void OnDayEnd(EventData data)
        {
            int currentDay = _timeManager != null ? _timeManager.DayCount : 0;
            var toFail = new List<NoticeBoardQuestData>();

            foreach (var q in _allQuests)
            {
                if (q.questState != QuestState.Active)   continue;
                if (q.timeLimitDays < 0)                 continue;
                if (currentDay - q.acceptedDay > q.timeLimitDays)
                    toFail.Add(q);
            }

            foreach (var q in toFail)
                TransitionToFailed(q);
        }

        // ── 狀態機轉移 ────────────────────────────────────────────

        public void UnlockQuest(string questId)
        {
            var q = Find(questId);
            if (q == null) return;

            if (q.questState != QuestState.Unavailable)
            {
                Debug.LogWarning($"[NoticeBoardManager] UnlockQuest: 非法轉移 {q.questState} → Available，questId={questId}");
                return;
            }
            q.questState = QuestState.Available;
        }

        public void AcceptQuest(string questId)
        {
            var q = Find(questId);
            if (q == null) return;

            if (q.questState != QuestState.Available)
            {
                Debug.LogWarning($"[NoticeBoardManager] AcceptQuest: 非法轉移 {q.questState} → Active，questId={questId}");
                return;
            }

            q.questState        = QuestState.Active;
            q.acceptedDay       = _timeManager != null ? _timeManager.DayCount : 0;
            q.currentStepIndex  = 0;

            if (q.steps != null && q.steps.Count > 0)
                q.steps[0].stepState = StepState.Revealed;

            var evt = new EventData();
            evt.Set("questId", questId);
            EventManager.Instance.Publish(GameEvents.ON_NOTICE_QUEST_ACCEPTED, evt);
        }

        public void CompleteStep(string questId, int stepIndex)
        {
            var q = Find(questId);
            if (q == null) return;

            if (q.questState != QuestState.Active)
            {
                Debug.LogWarning($"[NoticeBoardManager] CompleteStep: 委託 {questId} 不在 Active 狀態。");
                return;
            }

            if (q.steps == null || stepIndex >= q.steps.Count) return;

            var step = q.steps[stepIndex];
            if (step.stepState != StepState.Revealed)
            {
                Debug.LogWarning($"[NoticeBoardManager] CompleteStep: 步驟 {stepIndex} 不在 Revealed 狀態，questId={questId}");
                return;
            }

            step.stepState      = StepState.Completed;
            q.currentStepIndex  = stepIndex + 1;

            bool isLastStep = (q.currentStepIndex >= q.steps.Count);

            if (!isLastStep)
            {
                q.steps[q.currentStepIndex].stepState = StepState.Revealed;

                var revealEvt = new EventData();
                revealEvt.Set("questId",   questId);
                revealEvt.Set("stepIndex", q.currentStepIndex);
                EventManager.Instance.Publish(GameEvents.ON_NOTICE_QUEST_STEP_REVEALED, revealEvt);
            }
            else
            {
                TransitionToCompleted(q);
            }
        }

        private void TransitionToCompleted(NoticeBoardQuestData q)
        {
            q.questState = QuestState.Completed;

            var evt = new EventData();
            evt.Set("questId", q.questId);
            EventManager.Instance.Publish(GameEvents.ON_NOTICE_QUEST_COMPLETED, evt);

            TriggerConsequence(q);
        }

        private void TransitionToFailed(NoticeBoardQuestData q)
        {
            q.questState = QuestState.Failed;

            var evt = new EventData();
            evt.Set("questId", q.questId);
            EventManager.Instance.Publish(GameEvents.ON_NOTICE_QUEST_FAILED, evt);

            TriggerConsequence(q);
        }

        private void TriggerConsequence(NoticeBoardQuestData q)
        {
            if (string.IsNullOrEmpty(q.consequenceKey)) return;

            var evt = new EventData();
            evt.Set("questId",        q.questId);
            evt.Set("consequenceKey", q.consequenceKey);
            EventManager.Instance.Publish(GameEvents.ON_NOTICE_QUEST_CONSEQUENCE, evt);
        }

        // ── 對外接口 ─────────────────────────────────────────────

        public List<NoticeBoardQuestData> GetQuestsByTown(string townId) =>
            _allQuests.FindAll(q => q.townId == townId && q.questState == QuestState.Available);

        public List<NoticeBoardQuestData> GetActiveQuests() =>
            _allQuests.FindAll(q => q.questState == QuestState.Active);

        public NoticeBoardSaveData CaptureState() =>
            new NoticeBoardSaveData { quests = new List<NoticeBoardQuestData>(_allQuests) };

        public void RestoreState(NoticeBoardSaveData saveData)
        {
            if (saveData?.quests == null) return;
            _allQuests.Clear();
            _allQuests.AddRange(saveData.quests);
        }

        private NoticeBoardQuestData Find(string questId) =>
            _allQuests.Find(q => q.questId == questId);
    }
}
