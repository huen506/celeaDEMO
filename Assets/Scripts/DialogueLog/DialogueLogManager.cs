using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    public enum LogDisplayMode { Normal, Immersive }

    [System.Serializable]
    public class LogSaveData
    {
        public List<DialogueLogEntry> entries = new List<DialogueLogEntry>();
        public LogDisplayMode displayMode;
    }

    public class DialogueLogManager : MonoBehaviour
    {
        private const int MAX_LOG_ENTRIES = 500;

        [Header("元件（在 Inspector 指定）")]
        [SerializeField] private TimeManager _timeManager;

        private readonly List<DialogueLogEntry> _entries = new List<DialogueLogEntry>();
        private LogDisplayMode _displayMode = LogDisplayMode.Normal;
        private int _entryCounter = 0;

        public LogDisplayMode DisplayMode => _displayMode;

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_DIALOGUE_LINE_PLAYED, OnDialogueLinePlayed);
            EventManager.Instance.Subscribe(GameEvents.ON_CHOICE_MADE,          OnChoiceMade);
            EventManager.Instance.Subscribe(GameEvents.ON_HIDDEN_QUEST_TRIGGERED, OnHiddenQuestTriggered);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_DIALOGUE_LINE_PLAYED, OnDialogueLinePlayed);
            EventManager.Instance.Unsubscribe(GameEvents.ON_CHOICE_MADE,          OnChoiceMade);
            EventManager.Instance.Unsubscribe(GameEvents.ON_HIDDEN_QUEST_TRIGGERED, OnHiddenQuestTriggered);
        }

        // ── 事件接收 ─────────────────────────────────────────────

        private void OnDialogueLinePlayed(EventData data)
        {
            bool isInnerThought = data.Get<bool>("isInnerThought");
            AddEntry(new DialogueLogEntry
            {
                speakerId          = data.Get<string>("speakerId"),
                speakerDisplayName = data.Get<string>("speakerDisplayName"),
                text               = data.Get<string>("text"),
                entryType          = isInnerThought ? LogEntryType.InnerThought : LogEntryType.Normal,
            });
        }

        private void OnChoiceMade(EventData data)
        {
            string payloadKey = data.Get<string>("payloadKey");
            AddEntry(new DialogueLogEntry
            {
                speakerId          = "",
                speakerDisplayName = "",
                text               = payloadKey,
                entryType          = LogEntryType.Choice,
            });
        }

        private void OnHiddenQuestTriggered(EventData data)
        {
            string characterId = data.Get<string>("characterId");
            if (string.IsNullOrEmpty(characterId)) return;

            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                if (_entries[i].speakerId == characterId)
                {
                    _entries[i].hasImplicitConsequence = true;
                    return;
                }
            }
            // 找不到對應紀錄，靜默跳過
        }

        // ── 內部 ─────────────────────────────────────────────────

        private void AddEntry(DialogueLogEntry entry)
        {
            _entryCounter++;
            entry.entryId   = $"LOG_{_entryCounter:D6}";
            entry.timestamp = BuildTimestamp();
            _entries.Add(entry);
            TrimIfNeeded();
        }

        private string BuildTimestamp()
        {
            if (_timeManager == null) return "";
            return $"第{_timeManager.DayCount}日_{_timeManager.CurrentSlot}";
        }

        private void TrimIfNeeded()
        {
            if (_entries.Count <= MAX_LOG_ENTRIES) return;

            for (int i = 0; i < _entries.Count; i++)
            {
                if (!_entries[i].isMarked)
                {
                    _entries.RemoveAt(i);
                    return;
                }
            }
            // 全部是標記紀錄，不截斷
        }

        // ── 對外接口 ─────────────────────────────────────────────

        public List<DialogueLogEntry> GetAllEntries()    => new List<DialogueLogEntry>(_entries);
        public List<DialogueLogEntry> GetMarkedEntries() =>
            _entries.FindAll(e => e.isMarked);

        public void ToggleMark(string entryId)
        {
            var entry = _entries.Find(e => e.entryId == entryId);
            if (entry != null) entry.isMarked = !entry.isMarked;
        }

        public void SetDisplayMode(LogDisplayMode mode)
        {
            _displayMode = mode;
        }

        public LogSaveData CaptureState()
        {
            return new LogSaveData
            {
                entries     = new List<DialogueLogEntry>(_entries),
                displayMode = _displayMode,
            };
        }

        public void RestoreState(LogSaveData saveData)
        {
            if (saveData == null) return;
            _entries.Clear();
            if (saveData.entries != null)
                _entries.AddRange(saveData.entries);
            _displayMode = saveData.displayMode;
        }
    }
}
