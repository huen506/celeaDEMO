using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Celea
{
    /// <summary>
    /// 圖鑑 UI。
    /// 入口：選單 → 紀錄 → 圖鑑。
    /// 格狀介面 + 全螢幕查閱 + 左右切換。
    /// </summary>
    public class GalleryUI : MonoBehaviour
    {
        [Header("元件（在 Inspector 指定）")]
        [SerializeField] private GalleryManager _manager;

        [Header("格狀介面")]
        [SerializeField] private GameObject  _galleryPanel;
        [SerializeField] private Transform   _gridContainer;
        [SerializeField] private GameObject  _gridItemPrefab;
        [SerializeField] private TextMeshProUGUI _emptyLabel;

        [Header("全螢幕查閱")]
        [SerializeField] private GameObject  _fullscreenPanel;
        [SerializeField] private RawImage    _fullscreenImage;
        [SerializeField] private Button      _prevButton;
        [SerializeField] private Button      _nextButton;

        private List<GalleryEntryData> _unlockedEntries = new List<GalleryEntryData>();
        private int _currentFullscreenIndex = 0;

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_GALLERY_UNLOCKED, OnGalleryUnlocked);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_GALLERY_UNLOCKED, OnGalleryUnlocked);
        }

        private void Start()
        {
            if (_prevButton != null) _prevButton.onClick.AddListener(ShowPrev);
            if (_nextButton != null) _nextButton.onClick.AddListener(ShowNext);
            if (_fullscreenPanel != null)
            {
                var bg = _fullscreenPanel.GetComponent<Button>();
                if (bg != null) bg.onClick.AddListener(CloseFullscreen);
            }
        }

        // ── 事件接收 ─────────────────────────────────────────────

        private void OnGalleryUnlocked(EventData data)
        {
            if (_galleryPanel != null && _galleryPanel.activeSelf)
                RefreshGrid();
        }

        // ── 公開入口 ─────────────────────────────────────────────

        public void OpenGallery()
        {
            if (_galleryPanel != null) _galleryPanel.SetActive(true);
            RefreshGrid();
        }

        public void CloseGallery()
        {
            if (_galleryPanel != null) _galleryPanel.SetActive(false);
        }

        // ── 格狀介面 ─────────────────────────────────────────────

        private void RefreshGrid()
        {
            if (_gridContainer == null || _gridItemPrefab == null || _manager == null) return;

            foreach (Transform child in _gridContainer)
                Destroy(child.gameObject);

            var all = _manager.GetAllEntries();
            _unlockedEntries = _manager.GetUnlockedEntries();

            if (_emptyLabel != null)
                _emptyLabel.gameObject.SetActive(_unlockedEntries.Count == 0);

            foreach (var entry in all)
            {
                var go   = Instantiate(_gridItemPrefab, _gridContainer);
                var img  = go.GetComponentInChildren<RawImage>();
                var btn  = go.GetComponent<Button>();

                if (entry.isUnlocked)
                {
                    var tex = Resources.Load<Texture2D>(entry.resourceKey);
                    if (tex != null && img != null)
                        img.texture = tex;
                    else if (img != null)
                        Debug.LogError($"[GalleryUI] 圖片載入失敗：{entry.resourceKey}");

                    if (btn != null)
                    {
                        var captured = entry;
                        btn.onClick.AddListener(() => OpenFullscreen(captured));
                    }
                }
                else
                {
                    // 未解鎖：灰色佔位（佔位，待美術端定義樣式）
                    if (img != null) img.color = new Color(0.3f, 0.3f, 0.3f, 1f);
                    if (btn != null) btn.interactable = false;
                }
            }
        }

        // ── 全螢幕查閱 ────────────────────────────────────────────

        private void OpenFullscreen(GalleryEntryData entry)
        {
            _currentFullscreenIndex = _unlockedEntries.IndexOf(entry);
            if (_currentFullscreenIndex < 0) return;

            if (_fullscreenPanel != null) _fullscreenPanel.SetActive(true);
            ShowFullscreenAt(_currentFullscreenIndex);
        }

        public void CloseFullscreen()
        {
            if (_fullscreenPanel != null) _fullscreenPanel.SetActive(false);
        }

        private void ShowPrev()
        {
            if (_currentFullscreenIndex <= 0) return;
            _currentFullscreenIndex--;
            ShowFullscreenAt(_currentFullscreenIndex);
        }

        private void ShowNext()
        {
            if (_currentFullscreenIndex >= _unlockedEntries.Count - 1) return;
            _currentFullscreenIndex++;
            ShowFullscreenAt(_currentFullscreenIndex);
        }

        private void ShowFullscreenAt(int index)
        {
            if (index < 0 || index >= _unlockedEntries.Count) return;

            var entry = _unlockedEntries[index];
            var tex   = Resources.Load<Texture2D>(entry.resourceKey);

            if (_fullscreenImage != null)
            {
                if (tex != null)
                    _fullscreenImage.texture = tex;
                else
                    Debug.LogError($"[GalleryUI] 全螢幕圖片載入失敗：{entry.resourceKey}");
            }

            if (_prevButton != null) _prevButton.interactable = (index > 0);
            if (_nextButton != null) _nextButton.interactable = (index < _unlockedEntries.Count - 1);
        }
    }
}
