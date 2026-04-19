using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Celea
{
    /// <summary>
    /// 對話履歷 UI。
    /// 掛載在 HUDLayer 下，常駐左下角。
    /// 劇情中可疊加顯示在 DialogueLayer 上方。
    /// </summary>
    public class DialogueLogUI : MonoBehaviour
    {
        [Header("元件（在 Inspector 指定）")]
        [SerializeField] private DialogueLogManager _logManager;

        [Header("HUD 履歷面板")]
        [SerializeField] private GameObject   _logPanel;
        [SerializeField] private Transform    _entryContainer;
        [SerializeField] private GameObject   _entryPrefab;
        [SerializeField] private Button       _toggleButton;

        [Header("劇情中疊加層")]
        [SerializeField] private GameObject   _overlayPanel;
        [SerializeField] private Transform    _overlayContainer;
        [SerializeField] private Button       _logDialogueButton;

        // 顏色佔位常數（待美術端定義後替換）
        private static readonly Color COLOR_NORMAL              = Color.white;
        private static readonly Color COLOR_IMPLICIT_CONSEQUENCE = new Color(1f, 0.85f, 0.4f);

        private bool _isPanelExpanded = true;
        private bool _isOverlayOpen   = false;

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_LOG_TOGGLE_REQUESTED, OnLogToggleRequested);
            EventManager.Instance.Subscribe(GameEvents.ON_DIALOGUE_LINE_PLAYED, OnDialogueLinePlayed);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_LOG_TOGGLE_REQUESTED, OnLogToggleRequested);
            EventManager.Instance.Unsubscribe(GameEvents.ON_DIALOGUE_LINE_PLAYED, OnDialogueLinePlayed);
        }

        private void Start()
        {
            if (_toggleButton != null)
                _toggleButton.onClick.AddListener(TogglePanel);

            if (_logDialogueButton != null)
                _logDialogueButton.onClick.AddListener(ToggleOverlay);

            RefreshPanel();
        }

        // ── 事件接收 ─────────────────────────────────────────────

        private void OnLogToggleRequested(EventData data) => ToggleOverlay();

        private void OnDialogueLinePlayed(EventData data)
        {
            RefreshPanel();
            if (_isOverlayOpen) RefreshOverlay();
        }

        // ── 面板控制 ─────────────────────────────────────────────

        public void TogglePanel()
        {
            _isPanelExpanded = !_isPanelExpanded;
            if (_logPanel != null) _logPanel.SetActive(_isPanelExpanded);
        }

        public void ToggleOverlay()
        {
            _isOverlayOpen = !_isOverlayOpen;
            if (_overlayPanel != null) _overlayPanel.SetActive(_isOverlayOpen);
            if (_isOverlayOpen) RefreshOverlay();
        }

        public void CloseOverlay()
        {
            if (!_isOverlayOpen) return;
            _isOverlayOpen = false;
            if (_overlayPanel != null) _overlayPanel.SetActive(false);
        }

        // ── 渲染 ─────────────────────────────────────────────────

        private void RefreshPanel()
        {
            if (_entryContainer == null || _entryPrefab == null || _logManager == null) return;
            RenderEntries(_entryContainer, _logManager.GetAllEntries());
        }

        private void RefreshOverlay()
        {
            if (_overlayContainer == null || _entryPrefab == null || _logManager == null) return;
            RenderEntries(_overlayContainer, _logManager.GetAllEntries());
        }

        private void RenderEntries(Transform container, List<DialogueLogEntry> entries)
        {
            foreach (Transform child in container)
                Destroy(child.gameObject);

            bool immersive = _logManager != null &&
                             _logManager.DisplayMode == LogDisplayMode.Immersive;

            foreach (var entry in entries)
            {
                var go = Instantiate(_entryPrefab, container);

                var nameLabel = go.transform.Find("NameLabel")?.GetComponent<TextMeshProUGUI>();
                var textLabel = go.transform.Find("TextLabel")?.GetComponent<TextMeshProUGUI>();
                var markBtn   = go.transform.Find("MarkButton")?.GetComponent<Button>();

                if (nameLabel != null) nameLabel.text = entry.speakerDisplayName;
                if (textLabel  != null)
                {
                    textLabel.text  = entry.text;
                    textLabel.color = (!immersive && entry.hasImplicitConsequence)
                        ? COLOR_IMPLICIT_CONSEQUENCE
                        : COLOR_NORMAL;
                }

                if (markBtn != null)
                {
                    string id = entry.entryId;
                    markBtn.onClick.AddListener(() =>
                    {
                        _logManager.ToggleMark(id);
                        RefreshPanel();
                        if (_isOverlayOpen) RefreshOverlay();
                    });
                }
            }
        }
    }
}
