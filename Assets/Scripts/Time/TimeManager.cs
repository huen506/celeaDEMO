using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 時間系統主系統。
    /// 管理時段狀態、推進邏輯、跨日流程、發出時間事件。
    /// 不直接呼叫對話系統，只透過事件通信。
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        [Header("元件（在 Inspector 指定）")]
        [SerializeField] private StepCounter _stepCounter;

        private TimeSlot _currentSlot          = TimeSlot.Dawn;
        private int      _dayCount             = 1;
        private int      _consecutiveNoRest    = 0;

        // 時段推進順序（不包含 Midnight，Midnight 是特殊狀態）
        private static readonly TimeSlot[] SLOT_ORDER =
        {
            TimeSlot.Dawn,
            TimeSlot.Noon,
            TimeSlot.Dusk,
            TimeSlot.Night
        };

        private void Awake()
        {
            if (_stepCounter != null)
                _stepCounter.OnStepThresholdReached = OnStepThresholdReached;
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_DAY_END_DECISION, OnDayEndDecision);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_DAY_END_DECISION, OnDayEndDecision);
        }

        // ── 時段推進 ─────────────────────────────────────────────

        /// <summary>
        /// 推進一個時段。
        /// 若已在 Night → 發出跨日提示；若已在 Midnight → 直接跨日。
        /// </summary>
        public void AdvanceSlot()
        {
            if (_currentSlot == TimeSlot.Night)
            {
                RequestDayEnd();
                return;
            }

            if (_currentSlot == TimeSlot.Midnight)
            {
                ProcessDayEnd(rested: false);
                return;
            }

            int nextIndex = System.Array.IndexOf(SLOT_ORDER, _currentSlot) + 1;
            if (nextIndex < SLOT_ORDER.Length)
            {
                _currentSlot = SLOT_ORDER[nextIndex];
                PublishTimeProgress();
            }
        }

        /// <summary>
        /// 出城探索：每兩個節點消耗一個時段。
        /// 呼叫方（PlayerController）自行計算兩個節點後呼叫一次。
        /// </summary>
        public void ConsumeSlotForTravel()
        {
            AdvanceSlot();
        }

        // ── 跨日機制 ─────────────────────────────────────────────

        private void RequestDayEnd()
        {
            var data = new EventData();
            data.Set("consecutiveNoRestDays", _consecutiveNoRest);
            EventManager.Instance.Publish(GameEvents.ON_DAY_END_PROMPT_REQUESTED, data);
        }

        private void OnDayEndDecision(EventData data)
        {
            string decision = data.Get<string>("decision");

            if (decision == "rest")
            {
                ProcessDayEnd(rested: true);
            }
            else if (decision == "continue")
            {
                _currentSlot = TimeSlot.Midnight;
                PublishTimeProgress();
            }
            else
            {
                Debug.LogWarning($"[TimeManager] 未知的 decision 值：{decision}");
            }
        }

        private void ProcessDayEnd(bool rested)
        {
            if (rested)
                _consecutiveNoRest = 0;
            else
                _consecutiveNoRest++;

            _dayCount++;
            _currentSlot = TimeSlot.Dawn;

            _stepCounter?.Reset();

            var data = new EventData();
            data.Set("rested",                  rested);
            data.Set("consecutiveNoRestDays",   _consecutiveNoRest);
            data.Set("dayCount",                _dayCount);
            EventManager.Instance.Publish(GameEvents.ON_DAY_END, data);

            // 跨日後發出時段推進事件（進入清晨）
            PublishTimeProgress();
        }

        // ── StepCounter 回調 ────────────────────────────────────

        private void OnStepThresholdReached()
        {
            AdvanceSlot();
        }

        // ── 主線事件觸發接口 ────────────────────────────────────

        /// <summary>觸發主線事件，攜帶事件 ID 發出 ON_MAIN_EVENT。</summary>
        public void TriggerMainEvent(string eventId)
        {
            var data = new EventData();
            data.Set("eventId", eventId);
            EventManager.Instance.Publish(GameEvents.ON_MAIN_EVENT, data);
        }

        // ── 對外查詢 ─────────────────────────────────────────────

        public TimeSlot CurrentSlot              => _currentSlot;
        public int      DayCount                 => _dayCount;
        public int      ConsecutiveNoRestDays    => _consecutiveNoRest;

        // ── 存檔接口 ─────────────────────────────────────────────

        public TimeSaveData CaptureState()
        {
            return new TimeSaveData
            {
                currentSlot           = _currentSlot,
                dayCount              = _dayCount,
                consecutiveNoRestDays = _consecutiveNoRest
            };
        }

        public void RestoreState(TimeSaveData saveData)
        {
            _currentSlot        = saveData.currentSlot;
            _dayCount           = saveData.dayCount;
            _consecutiveNoRest  = saveData.consecutiveNoRestDays;
        }

        // ── 內部輔助 ─────────────────────────────────────────────

        private void PublishTimeProgress()
        {
            var data = new EventData();
            data.Set("slot", _currentSlot.ToString());
            EventManager.Instance.Publish(GameEvents.ON_TIME_PROGRESS, data);
            Debug.Log($"[TimeManager] 時段推進 → {_currentSlot}（第 {_dayCount} 天）");
        }
    }
}
