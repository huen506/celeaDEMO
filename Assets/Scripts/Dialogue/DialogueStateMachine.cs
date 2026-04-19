using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 對話系統的狀態列舉。
    /// </summary>
    public enum DialogueState
    {
        Idle,       // 待機，沒有對話在進行
        Playing,    // 正在播放當前句的打字機效果
        Waiting,    // 當前句播完，等待推進到下一句
        Choosing,   // 選擇自白進行中，等待玩家選擇
        Finishing   // 最後一句播完，準備發出 ON_DIALOGUE_END 事件
    }

    /// <summary>
    /// 純狀態管理。只管現在是哪個狀態、怎麼切換，不做任何流程邏輯。
    /// </summary>
    public class DialogueStateMachine
    {
        public DialogueState Current { get; private set; } = DialogueState.Idle;

        public void TransitionTo(DialogueState newState)
        {
            if (Current == newState) return;
            Debug.Log($"[DialogueStateMachine] {Current} → {newState}");
            Current = newState;
        }

        public bool Is(DialogueState state) => Current == state;
    }
}
