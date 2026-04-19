using UnityEngine;
using UnityEngine.UI;

namespace Celea
{
    /// <summary>
    /// 選單控制器。
    /// 管理選單的開關，以及依當前遊戲狀態決定存檔按鈕是否可用。
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [Header("存檔按鈕（佔位符）")]
        [SerializeField] private Button saveButton;

        public enum GameState
        {
            FreeRoam,   // 自由行動 - 存檔可用
            Dialogue,   // 劇情中 - 存檔不可用
            Exploring   // 探索場景中 - 存檔不可用
        }

        private GameState _currentGameState = GameState.FreeRoam;

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_UI_MODE_CHANGE, OnUIModeChange);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_UI_MODE_CHANGE, OnUIModeChange);
        }

        private void OnUIModeChange(EventData data)
        {
            UIManager.UIMode mode = data.Get<UIManager.UIMode>("mode");

            switch (mode)
            {
                case UIManager.UIMode.FreeRoam:
                    SetGameState(GameState.FreeRoam);
                    break;
                case UIManager.UIMode.Dialogue:
                    SetGameState(GameState.Dialogue);
                    break;
            }
        }

        public void SetGameState(GameState state)
        {
            _currentGameState = state;
            RefreshSaveButtonState();
        }

        private void RefreshSaveButtonState()
        {
            if (saveButton == null) return;

            bool canSave = _currentGameState == GameState.FreeRoam;
            saveButton.interactable = canSave;
        }

        // --- 選單按鈕回調（由 Inspector 指定） ---

        public void OnResumeClicked()
        {
            UIManager.Instance.CloseMenu();
        }

        public void OnSaveClicked()
        {
            if (_currentGameState != GameState.FreeRoam) return;

            EventData data = new EventData();
            EventManager.Instance.Publish(GameEvents.ON_SAVE_REQUESTED, data);
        }

        public void OnQuitClicked()
        {
            // 確認對話框細節待 UI 規格定義，此階段直接退出作為佔位符
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
