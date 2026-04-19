using UnityEngine;
using UnityEngine.UI;

namespace Celea
{
    /// <summary>
    /// 劇情層 UI 控制器。
    /// 管理全螢幕插圖、角色立繪區（最多四個位置）、對話框的展開與收起。
    /// 監聽 ON_UI_MODE_CHANGE 事件，由 UIManager 統一呼叫 Show/Hide。
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        [Header("插圖區（佔位符）")]
        [SerializeField] private Image backgroundImage;   // 全螢幕背景插圖

        [Header("角色立繪區 - 最多四個位置（佔位符）")]
        [SerializeField] private Image[] characterSlots = new Image[4];

        [Header("對話框（佔位符）")]
        [SerializeField] private GameObject dialogueBox;  // 固定在畫面下方
        [SerializeField] private Text speakerNameText;    // 說話者名稱
        [SerializeField] private Text dialogueBodyText;   // 對話內容

        /// <summary>
        /// 展開劇情層，顯示插圖、立繪、對話框。
        /// 由 UIManager 在切換至 Dialogue 模式時呼叫。
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 收起劇情層。
        /// 由 UIManager 在劇情結束後呼叫，並觸發 ON_DIALOGUE_END 事件。
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            EventManager.Instance.Publish(GameEvents.ON_DIALOGUE_END);
        }

        /// <summary>
        /// 更新對話框內容。
        /// </summary>
        public void SetDialogue(string speaker, string body)
        {
            if (speakerNameText != null) speakerNameText.text = speaker;
            if (dialogueBodyText != null) dialogueBodyText.text = body;
        }

        /// <summary>
        /// 設定指定位置的角色立繪（0 ~ 3）。
        /// 傳入 null 則清空該位置。
        /// </summary>
        public void SetCharacterSprite(int slotIndex, Sprite sprite)
        {
            if (slotIndex < 0 || slotIndex >= characterSlots.Length) return;
            if (characterSlots[slotIndex] == null) return;

            characterSlots[slotIndex].sprite = sprite;
            characterSlots[slotIndex].gameObject.SetActive(sprite != null);
        }

        /// <summary>
        /// 設定全螢幕背景插圖。
        /// </summary>
        public void SetBackground(Sprite bg)
        {
            if (backgroundImage != null)
                backgroundImage.sprite = bg;
        }
    }
}
