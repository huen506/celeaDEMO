using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

namespace Celea
{
    /// <summary>
    /// 四角常駐介面控制器。
    /// 負責更新 HUDLayer 內的時間、地點、地圖縮圖、角色立繪等資料顯示。
    /// 視覺細節（字體、色彩、動畫）由美術端後續定義，此階段使用佔位符元件。
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("左上角 - 時間與地點（佔位符）")]
        [SerializeField] private TextMeshProUGUI timeText;        // 當前時間
        [SerializeField] private TextMeshProUGUI dateText;        // 當前日期
        [SerializeField] private TextMeshProUGUI locationText;    // 當前地點名稱

        [Header("右下角 - 角色立繪（佔位符）")]
        [SerializeField] private Image characterPortrait;  // 角色立繪圖片

        // 角色立繪狀態對應（由規格書定義）
        [Header("角色立繪 Sprite 對應（佔位符）")]
        [SerializeField] private Sprite portraitNormal;       // 100% 正常
        [SerializeField] private Sprite portraitMinorDamage;  // 75%  輕微受損
        [SerializeField] private Sprite portraitMidDamage;    // 50%  中度受損
        [SerializeField] private Sprite portraitHeavyDamage;  // 25%  重度受損

        [Header("選單按鈕")]
        [SerializeField] private Button menuButton;

        [Header("時段換圖（佔位符）")]
        [SerializeField] private Image timeFrameImage;
        [SerializeField] private Sprite spriteDawn;
        [SerializeField] private Sprite spriteNoon;
        [SerializeField] private Sprite spriteDusk;
        [SerializeField] private Sprite spriteNight;
        [SerializeField] private Sprite spriteMidnight; // 預設拖入夜晚素材，之後可獨立替換

        private void Start()
        {
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuButtonClicked);
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_TIME_PROGRESS, OnTimeProgress);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_TIME_PROGRESS, OnTimeProgress);
        }

        private void OnDestroy()
        {
            if (menuButton != null)
                menuButton.onClick.RemoveListener(OnMenuButtonClicked);
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                ToggleMenuSafely();
        }

        private void OnMenuButtonClicked()
        {
            ToggleMenuSafely();
        }

        private void ToggleMenuSafely()
        {
            if (UIManager.Instance == null)
            {
                Debug.LogError("[HUDController] UIManager.Instance is null，無法操作選單。");
                return;
            }
            if (UIManager.Instance.CurrentMode == UIManager.UIMode.Menu)
                UIManager.Instance.CloseMenu();
            else
                UIManager.Instance.OpenMenu();
        }

        private void OnTimeProgress(EventData data)
        {
            if (timeFrameImage == null) return;
            switch (data.Get<string>("timePeriod"))
            {
                case "Dawn":     timeFrameImage.sprite = spriteDawn;     break;
                case "Noon":     timeFrameImage.sprite = spriteNoon;     break;
                case "Dusk":     timeFrameImage.sprite = spriteDusk;     break;
                case "Night":    timeFrameImage.sprite = spriteNight;    break;
                case "Midnight": timeFrameImage.sprite = spriteMidnight; break;
            }
        }

        /// <summary>
        /// 更新左上角時間與地點資訊。
        /// 由時間系統與場景管理系統呼叫。
        /// </summary>
        public void UpdateTimeAndLocation(string time, string date, string location)
        {
            if (timeText != null)     timeText.text     = time;
            if (dateText != null)     dateText.text     = date;
            if (locationText != null) locationText.text = location;
        }

        /// <summary>
        /// 更新右下角角色立繪。
        /// 根據生命百分比（0.0 ~ 1.0）切換對應立繪。
        /// </summary>
        public void UpdateCharacterPortrait(float hpPercent)
        {
            if (characterPortrait == null) return;

            if (hpPercent > 0.75f)
                characterPortrait.sprite = portraitNormal;
            else if (hpPercent > 0.50f)
                characterPortrait.sprite = portraitMinorDamage;
            else if (hpPercent > 0.25f)
                characterPortrait.sprite = portraitMidDamage;
            else
                characterPortrait.sprite = portraitHeavyDamage;
        }
    }
}
