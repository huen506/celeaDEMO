using UnityEngine;

namespace Celea
{
    /// <summary>
    /// UI 層管理主系統。
    /// 管理 HUDLayer、DialogueLayer、MenuLayer 的顯示與隱藏。
    /// 監聽 GameEvents.ON_UI_MODE_CHANGE 事件來切換模式。
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance => _instance;

        [Header("UI Layers - 由 Inspector 指定")]
        [SerializeField] private GameObject hudLayer;
        [SerializeField] private GameObject dialogueLayer;
        [SerializeField] private GameObject menuLayer;

        public enum UIMode
        {
            FreeRoam,   // 自由行動：顯示 HUD，隱藏 Dialogue 與 Menu
            Dialogue,   // 劇情中：隱藏 HUD，顯示 Dialogue
            Menu        // 選單開啟：顯示 Menu，其他不動
        }

        private UIMode _currentMode = UIMode.FreeRoam;
        public UIMode CurrentMode => _currentMode;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_UI_MODE_CHANGE, OnUIModeChange);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_UI_MODE_CHANGE, OnUIModeChange);
        }

        private void Start()
        {
            ApplyMode(UIMode.FreeRoam);
        }

        private void OnUIModeChange(EventData data)
        {
            UIMode newMode = data.Get<UIMode>("mode");
            ApplyMode(newMode);
        }

        public void ApplyMode(UIMode mode)
        {
            _currentMode = mode;

            switch (mode)
            {
                case UIMode.FreeRoam:
                    SetLayer(hudLayer, true);
                    SetLayer(dialogueLayer, false);
                    SetLayer(menuLayer, false);
                    break;

                case UIMode.Dialogue:
                    SetLayer(hudLayer, false);
                    SetLayer(dialogueLayer, true);
                    SetLayer(menuLayer, false);
                    break;

                case UIMode.Menu:
                    SetLayer(menuLayer, true);
                    // HUD 與 Dialogue 維持當前狀態，Menu 疊在最上層
                    break;
            }
        }

        public void OpenMenu()
        {
            ApplyMode(UIMode.Menu);
        }

        public void CloseMenu()
        {
            ApplyMode(UIMode.FreeRoam);
        }

        private void SetLayer(GameObject layer, bool visible)
        {
            if (layer != null)
                layer.SetActive(visible);
        }
    }
}
