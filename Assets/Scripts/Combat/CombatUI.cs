using UnityEngine;

namespace Celea
{
    // CombatUI 只監聽事件，不持有 CombatManager 引用，不推送資料回 CombatManager
    public class CombatUI : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_TURN_START, OnTurnStart);
            EventManager.Instance.Subscribe(GameEvents.ON_RESONANCE_TRIGGERED, OnResonanceTriggered);
            EventManager.Instance.Subscribe(GameEvents.ON_RESONANCE_ENDED, OnResonanceEnded);
            EventManager.Instance.Subscribe(GameEvents.ON_BATTLE_STATS_UPDATED, OnBattleStatsUpdated);
            EventManager.Instance.Subscribe(GameEvents.ON_COMBAT_END, OnCombatEnd);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_TURN_START, OnTurnStart);
            EventManager.Instance.Unsubscribe(GameEvents.ON_RESONANCE_TRIGGERED, OnResonanceTriggered);
            EventManager.Instance.Unsubscribe(GameEvents.ON_RESONANCE_ENDED, OnResonanceEnded);
            EventManager.Instance.Unsubscribe(GameEvents.ON_BATTLE_STATS_UPDATED, OnBattleStatsUpdated);
            EventManager.Instance.Unsubscribe(GameEvents.ON_COMBAT_END, OnCombatEnd);
        }

        private void OnTurnStart(EventData data)
        {
            // 佔位：顯示行動選單
            ShowActionMenu();
        }

        private void OnResonanceTriggered(EventData data)
        {
            // 顯示必殺技欄位
            ShowUltimateButton(true);
        }

        private void OnResonanceEnded(EventData data)
        {
            ShowUltimateButton(false);
        }

        private void OnBattleStatsUpdated(EventData data)
        {
            // 佔位：刷新數值顯示
        }

        private void OnCombatEnd(EventData data)
        {
            HideActionMenu();
        }

        private void ShowActionMenu()
        {
            // 佔位：待 UI 元件掛載後實作
        }

        private void HideActionMenu()
        {
            // 佔位
        }

        private void ShowUltimateButton(bool visible)
        {
            // 佔位：visible=true 顯示，false 隱藏
        }
    }
}
