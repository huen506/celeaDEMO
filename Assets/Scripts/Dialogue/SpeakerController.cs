using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 說話者立繪亮暗控制。說話者亮度正常，非說話者調暗。
    /// 暗化比例待美術端定義，此階段用佔位常數。
    /// </summary>
    public class SpeakerController : MonoBehaviour
    {
        // 佔位常數：待美術端定義後替換
        private const float FOCUS_ALPHA   = 1.0f;
        private const float UNFOCUS_ALPHA = 0.5f;

        // 最多支援 4 個立繪位置（索引 0～3）
        private const int PORTRAIT_SLOT_COUNT = 4;

        /// <summary>
        /// 根據 portraitFocusSlot 調整立繪亮暗。
        /// 呼叫時機：每句 DialogueLine 開始播放時。
        /// </summary>
        /// <param name="focusSlot">說話者的位置索引（0～3）；-1 代表無說話者（旁白）。</param>
        public void UpdateFocus(int focusSlot)
        {
            for (int i = 0; i < PORTRAIT_SLOT_COUNT; i++)
            {
                float alpha = (focusSlot < 0 || i == focusSlot) ? FOCUS_ALPHA : UNFOCUS_ALPHA;
                ApplyAlpha(i, alpha);
            }
        }

        /// <summary>
        /// 將所有立繪恢復至正常亮度（對話結束後呼叫）。
        /// </summary>
        public void ResetAll()
        {
            for (int i = 0; i < PORTRAIT_SLOT_COUNT; i++)
                ApplyAlpha(i, FOCUS_ALPHA);
        }

        private void ApplyAlpha(int slot, float alpha)
        {
            // 佔位實作：實際應取得 slot 對應的 SpriteRenderer / CanvasGroup 並設定透明度
            // 待美術端完成立繪節點後接入
            Debug.Log($"[SpeakerController] Slot {slot} alpha → {alpha}");
        }
    }
}
