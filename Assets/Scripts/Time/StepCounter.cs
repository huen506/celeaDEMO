using System;
using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 步數計數器。監聽 ON_PLAYER_STEP 事件累計步數，累積 20 步後通知 TimeManager 推進時段。
    /// </summary>
    public class StepCounter : MonoBehaviour
    {
        private const int STEPS_PER_SLOT = 20;

        private int _stepCount;

        /// <summary>累積到 20 步時觸發，TimeManager 綁定此回調。</summary>
        public Action OnStepThresholdReached;

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_PLAYER_STEP, OnPlayerStep);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_PLAYER_STEP, OnPlayerStep);
        }

        private void OnPlayerStep(EventData data)
        {
            _stepCount++;
            if (_stepCount >= STEPS_PER_SLOT)
            {
                _stepCount = 0;
                OnStepThresholdReached?.Invoke();
            }
        }

        /// <summary>重置步數（跨日時呼叫）。</summary>
        public void Reset()
        {
            _stepCount = 0;
        }

        public int CurrentStepCount => _stepCount;
    }
}
