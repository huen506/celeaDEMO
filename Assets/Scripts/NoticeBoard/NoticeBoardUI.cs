using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Celea
{
    /// <summary>
    /// 佈告欄任務 UI。
    /// 羊皮紙委託介面與任務欄顯示。
    /// </summary>
    public class NoticeBoardUI : MonoBehaviour
    {
        [Header("元件（在 Inspector 指定）")]
        [SerializeField] private NoticeBoardManager _manager;

        [Header("佈告欄面板")]
        [SerializeField] private GameObject   _boardPanel;
        [SerializeField] private Transform    _questListContainer;
        [SerializeField] private GameObject   _questListItemPrefab;

        [Header("羊皮紙面板")]
        [SerializeField] private GameObject   _parchmentPanel;
        [SerializeField] private TextMeshProUGUI _titleLabel;
        [SerializeField] private Transform    _stepContainer;
        [SerializeField] private GameObject   _stepItemPrefab;
        [SerializeField] private Button       _markButton;
        [SerializeField] private Button       _closeParchmentButton;

        [Header("任務欄面板（選單 → 任務）")]
        [SerializeField] private GameObject   _questLogPanel;
        [SerializeField] private Transform    _activeQuestContainer;
        [SerializeField] private GameObject   _activeQuestItemPrefab;

        private string _currentTownId;
        private NoticeBoardQuestData _currentQuest;

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_NOTICE_QUEST_STEP_REVEALED, OnStepRevealed);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_NOTICE_QUEST_STEP_REVEALED, OnStepRevealed);
        }

        private void Start()
        {
            if (_closeParchmentButton != null)
                _closeParchmentButton.onClick.AddListener(CloseParchment);
        }

        // ── 事件接收 ─────────────────────────────────────────────

        private void OnStepRevealed(EventData data)
        {
            if (_currentQuest == null) return;
            if (data.Get<string>("questId") != _currentQuest.questId) return;
            // 佔位：步驟顯現淡入效果，待美術端定義後替換
            RefreshParchment(_currentQuest);
        }

        // ── 公開入口 ─────────────────────────────────────────────

        /// <summary>玩家與亭內佈告欄互動時呼叫，傳入城鎮 ID。</summary>
        public void OpenBoard(string townId)
        {
            _currentTownId = townId;
            if (_boardPanel != null) _boardPanel.SetActive(true);
            RefreshQuestList();
        }

        public void CloseBoard()
        {
            if (_boardPanel != null) _boardPanel.SetActive(false);
        }

        /// <summary>選單 → 任務入口。</summary>
        public void OpenQuestLog()
        {
            if (_questLogPanel != null) _questLogPanel.SetActive(true);
            RefreshActiveQuestList();
        }

        public void CloseQuestLog()
        {
            if (_questLogPanel != null) _questLogPanel.SetActive(false);
        }

        // ── 渲染 ─────────────────────────────────────────────────

        private void RefreshQuestList()
        {
            if (_questListContainer == null || _questListItemPrefab == null || _manager == null) return;

            foreach (Transform child in _questListContainer)
                Destroy(child.gameObject);

            var quests = _manager.GetQuestsByTown(_currentTownId);
            foreach (var q in quests)
            {
                var go      = Instantiate(_questListItemPrefab, _questListContainer);
                var label   = go.GetComponentInChildren<TextMeshProUGUI>();
                var btn     = go.GetComponent<Button>();

                if (label != null) label.text = q.title;
                if (btn   != null)
                {
                    var captured = q;
                    btn.onClick.AddListener(() => OpenParchment(captured));
                }
            }
        }

        private void OpenParchment(NoticeBoardQuestData quest)
        {
            _currentQuest = quest;
            if (_boardPanel   != null) _boardPanel.SetActive(false);
            if (_parchmentPanel != null) _parchmentPanel.SetActive(true);
            RefreshParchment(quest);
        }

        private void CloseParchment()
        {
            if (_parchmentPanel != null) _parchmentPanel.SetActive(false);
            if (!string.IsNullOrEmpty(_currentTownId))
                OpenBoard(_currentTownId);
        }

        private void RefreshParchment(NoticeBoardQuestData quest)
        {
            if (quest == null) return;

            if (_titleLabel != null) _titleLabel.text = quest.title;

            if (_stepContainer != null && _stepItemPrefab != null)
            {
                foreach (Transform child in _stepContainer)
                    Destroy(child.gameObject);

                if (quest.steps != null)
                {
                    foreach (var step in quest.steps)
                    {
                        if (step.stepState == StepState.Locked) continue;

                        var go    = Instantiate(_stepItemPrefab, _stepContainer);
                        var label = go.GetComponentInChildren<TextMeshProUGUI>();
                        if (label != null) label.text = step.description;
                    }
                }
            }

            if (_markButton != null)
            {
                _markButton.onClick.RemoveAllListeners();
                _markButton.onClick.AddListener(() =>
                {
                    quest.isMarked = !quest.isMarked;
                });
            }
        }

        private void RefreshActiveQuestList()
        {
            if (_activeQuestContainer == null || _activeQuestItemPrefab == null || _manager == null) return;

            foreach (Transform child in _activeQuestContainer)
                Destroy(child.gameObject);

            var quests = _manager.GetActiveQuests();
            foreach (var q in quests)
            {
                var go    = Instantiate(_activeQuestItemPrefab, _activeQuestContainer);
                var label = go.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                    label.text = q.isMarked ? $"★ {q.title}" : q.title;
            }
        }
    }
}
