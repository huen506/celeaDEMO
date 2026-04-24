using System;
using System.Collections.Generic;
using UnityEngine;


namespace Celea
{
    /// <summary>
    /// 選擇自白控制器。通知 UI 暗化、顯示選項、回傳選擇結果給 FlowController。
    /// </summary>
    public class ChoiceController : MonoBehaviour
    {
        /// <summary>FlowController 綁定此回調以接收玩家選擇結果。</summary>
        public Action<DialogueChoice> OnChoiceSelected;

        /// <summary>顯示選擇自白。由 DialogueManager 在進入 Choosing 狀態時呼叫。</summary>
        public void ShowChoices(List<DialogueChoice> choices)
        {
            // 通知 UIManager 維持對話模式（選項是對話流程的一部分）
            var enterData = new EventData();
            enterData.Set("mode", UIManager.UIMode.Dialogue);
            EventManager.Instance.Publish(GameEvents.ON_UI_MODE_CHANGE, enterData);

            // 佔位：實際 UI 顯示由 UIManager / DialogueUI 層負責
            // ChoiceController 持有選項清單，等待 UIManager 呼叫 Select() 回報結果
            _pendingChoices = choices;

            Debug.Log($"[ChoiceController] 顯示 {choices.Count} 個選項，等待玩家選擇。");
        }

        /// <summary>
        /// UIManager 或按鈕事件呼叫此方法回報玩家選擇。
        /// index 為 choices 清單的索引（0～2）。
        /// </summary>
        public void Select(int index)
        {
            if (_pendingChoices == null || index < 0 || index >= _pendingChoices.Count)
            {
                Debug.LogError($"[ChoiceController] 無效的選擇索引：{index}");
                return;
            }

            var choice = _pendingChoices[index];
            _pendingChoices = null;

            // 通知 UIManager 維持對話模式（繼續對話流程）
            var exitData = new EventData();
            exitData.Set("mode", UIManager.UIMode.Dialogue);
            EventManager.Instance.Publish(GameEvents.ON_UI_MODE_CHANGE, exitData);

            OnChoiceSelected?.Invoke(choice);
        }

        private List<DialogueChoice> _pendingChoices;
    }
}
