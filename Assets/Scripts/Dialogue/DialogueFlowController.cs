using System;
using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 流程邏輯。決定播哪句、判斷 hasChoice、處理 nextSegmentId、通知事件。
    /// 不操作 UI，只透過事件通知外部。
    /// </summary>
    public class DialogueFlowController
    {
        private DialogueStateMachine _stateMachine;
        private DialogueData _currentData;
        private DialogueSegment _currentSegment;
        private int _currentLineIndex;

        // 外部需要綁定的回調
        public Action<DialogueLine> OnLineReady;       // 有新的一句話要播放
        public Action<DialogueSegment> OnChoiceReady;  // 有選擇自白要顯示
        public Action OnDialogueEnd;                   // 整段對話結束

        public DialogueFlowController(DialogueStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        /// <summary>開始播放一份對話資料。</summary>
        public void StartDialogue(DialogueData data)
        {
            _currentData = data;
            _currentLineIndex = 0;
            _currentSegment = data.FindSegment(data.startSegmentId);

            if (_currentSegment == null)
            {
                Debug.LogError($"[DialogueFlowController] 找不到起始段落 '{data.startSegmentId}'，對話終止。");
                return;
            }

            PlayCurrentLine();
        }

        /// <summary>玩家點擊推進（TypewriterEffect 完成後或玩家第二次點擊時呼叫）。</summary>
        public void Advance()
        {
            if (_stateMachine.Is(DialogueState.Choosing)) return; // 選擇中不響應

            if (_stateMachine.Is(DialogueState.Playing)) return;  // 打字機還在跑，由 TypewriterEffect 自行處理

            if (_stateMachine.Is(DialogueState.Waiting))
            {
                _currentLineIndex++;
                if (_currentLineIndex < _currentSegment.lines.Count)
                {
                    PlayCurrentLine();
                }
                else
                {
                    OnSegmentEnd();
                }
            }
        }

        /// <summary>打字機效果完成後呼叫，切換到 Waiting 狀態。</summary>
        public void OnTypewriterComplete()
        {
            _stateMachine.TransitionTo(DialogueState.Waiting);
        }

        /// <summary>ChoiceController 回傳玩家選擇結果後呼叫。</summary>
        public void OnChoiceSelected(DialogueChoice choice)
        {
            var choiceData = new EventData();
            choiceData.Set("choiceCategory", choice.choiceCategory.ToString());
            choiceData.Set("payloadKey", choice.payloadKey);
            EventManager.Instance.Publish(GameEvents.ON_CHOICE_MADE, choiceData);

            _stateMachine.TransitionTo(DialogueState.Waiting);
            MoveToNextSegment();
        }

        private void PlayCurrentLine()
        {
            _stateMachine.TransitionTo(DialogueState.Playing);
            var line = _currentSegment.lines[_currentLineIndex];
            OnLineReady?.Invoke(line);

            var lineData = new EventData();
            lineData.Set("speakerId", line.speakerId);
            lineData.Set("speakerDisplayName", line.speakerDisplayName);
            lineData.Set("text", line.text);
            lineData.Set("isInnerThought", line.isInnerThought);
            EventManager.Instance.Publish(GameEvents.ON_DIALOGUE_LINE_PLAYED, lineData);
        }

        private void OnSegmentEnd()
        {
            if (_currentSegment.hasChoice)
            {
                _stateMachine.TransitionTo(DialogueState.Choosing);
                OnChoiceReady?.Invoke(_currentSegment);
                return;
            }

            MoveToNextSegment();
        }

        private void MoveToNextSegment()
        {
            string nextId = _currentSegment.nextSegmentId;

            if (string.IsNullOrEmpty(nextId))
            {
                FinishDialogue();
                return;
            }

            var nextSeg = _currentData.FindSegment(nextId);
            if (nextSeg == null)
            {
                Debug.LogError($"[DialogueFlowController] 找不到下一段落 '{nextId}'，對話強制結束。");
                FinishDialogue();
                return;
            }

            _currentSegment = nextSeg;
            _currentLineIndex = 0;
            PlayCurrentLine();
        }

        private void FinishDialogue()
        {
            _stateMachine.TransitionTo(DialogueState.Finishing);
            OnDialogueEnd?.Invoke();
            _stateMachine.TransitionTo(DialogueState.Idle);
        }

        public DialogueLine GetCurrentLine()
        {
            if (_currentSegment == null || _currentLineIndex >= _currentSegment.lines.Count)
                return null;
            return _currentSegment.lines[_currentLineIndex];
        }
    }
}
