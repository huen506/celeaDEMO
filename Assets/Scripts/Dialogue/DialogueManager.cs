using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 對話系統薄接口層。
    /// 接收外部事件、協調 StateMachine 和 FlowController、對外提供查詢。
    /// 自己不做具體邏輯，不直接操作 UI。
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        [Header("功能層元件（在 Inspector 指定）")]
        [SerializeField] private TypewriterEffect _typewriterEffect;
        [SerializeField] private ChoiceController _choiceController;
        [SerializeField] private SpeakerController _speakerController;

        private DialogueStateMachine _stateMachine;
        private DialogueFlowController _flowController;

        private void Awake()
        {
            _stateMachine   = new DialogueStateMachine();
            _flowController = new DialogueFlowController(_stateMachine);

            // 綁定 FlowController 的回調
            _flowController.OnLineReady    = HandleLineReady;
            _flowController.OnChoiceReady  = HandleChoiceReady;
            _flowController.OnDialogueEnd  = HandleDialogueEnd;

            // 綁定 TypewriterEffect 的回調
            if (_typewriterEffect != null)
                _typewriterEffect.OnTypingComplete = _flowController.OnTypewriterComplete;

            // 綁定 ChoiceController 的回調
            if (_choiceController != null)
                _choiceController.OnChoiceSelected = _flowController.OnChoiceSelected;
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_DIALOGUE_START, OnDialogueStartEvent);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_DIALOGUE_START, OnDialogueStartEvent);
        }

        // ── 事件接收 ────────────────────────────────────────────

        private void OnDialogueStartEvent(EventData data)
        {
            string dialogueId = data.Get<string>("dialogueId");
            if (string.IsNullOrEmpty(dialogueId))
            {
                Debug.LogError("[DialogueManager] ON_DIALOGUE_START 事件缺少 dialogueId。");
                return;
            }

            StartDialogue(dialogueId);
        }

        // ── 公開接口 ─────────────────────────────────────────────

        /// <summary>從 Resources/Dialogues/ 載入並開始播放對話。</summary>
        public void StartDialogue(string dialogueId)
        {
            if (!_stateMachine.Is(DialogueState.Idle))
            {
                Debug.LogWarning("[DialogueManager] 對話進行中，忽略新的 StartDialogue 呼叫。");
                return;
            }

            TextAsset jsonAsset = Resources.Load<TextAsset>($"Dialogues/{dialogueId}");
            if (jsonAsset == null)
            {
                Debug.LogError($"[DialogueManager] 找不到對話資料：Dialogues/{dialogueId}");
                return;
            }

            DialogueData data = JsonUtility.FromJson<DialogueData>(jsonAsset.text);
            if (data == null)
            {
                Debug.LogError($"[DialogueManager] 解析對話 JSON 失敗：{dialogueId}");
                return;
            }

            // 通知 UIManager 切換到對話模式
            var modeData = new EventData();
            modeData.Set("mode", UIManager.UIMode.Dialogue);
            EventManager.Instance.Publish(GameEvents.ON_UI_MODE_CHANGE, modeData);

            _flowController.StartDialogue(data);
        }

        /// <summary>玩家點擊畫面推進對話時呼叫。</summary>
        public void OnPlayerTap()
        {
            if (_stateMachine.Is(DialogueState.Choosing)) return;

            if (_typewriterEffect != null && _typewriterEffect.IsTyping)
            {
                // 第一次點擊：中斷打字機，顯示全文
                _typewriterEffect.TryInterrupt();
                // 中斷後切換到 Waiting，等待第二次點擊推進
                _flowController.OnTypewriterComplete();
            }
            else
            {
                // 第二次點擊：推進到下一句
                _flowController.Advance();
            }
        }

        /// <summary>查詢目前是否有對話在進行中。</summary>
        public bool IsDialogueActive => !_stateMachine.Is(DialogueState.Idle);

        /// <summary>查詢目前的對話狀態。</summary>
        public DialogueState CurrentState => _stateMachine.Current;

        // ── FlowController 回調處理 ──────────────────────────────

        private void HandleLineReady(DialogueLine line)
        {
            // 通知 SpeakerController 更新立繪亮暗
            _speakerController?.UpdateFocus(line.portraitFocusSlot);

            // 通知 TypewriterEffect 開始打字
            _typewriterEffect?.Play(line.text);
        }

        private void HandleChoiceReady(DialogueSegment segment)
        {
            _choiceController?.ShowChoices(segment.choices);
        }

        private void HandleDialogueEnd()
        {
            // 重置立繪亮暗
            _speakerController?.ResetAll();

            // 通知外部對話結束
            EventManager.Instance.Publish(GameEvents.ON_DIALOGUE_END);

            // 通知 UIManager 切換回自由行動模式
            var modeData = new EventData();
            modeData.Set("mode", UIManager.UIMode.FreeRoam);
            EventManager.Instance.Publish(GameEvents.ON_UI_MODE_CHANGE, modeData);
        }
    }
}
