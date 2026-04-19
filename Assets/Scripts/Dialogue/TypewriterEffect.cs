using System;
using System.Collections;
using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 打字機效果。逐字顯示、點擊中斷、無配音佔位計時。
    /// 掛在負責顯示文字的 GameObject 上，或由 DialogueManager 持有。
    /// </summary>
    public class TypewriterEffect : MonoBehaviour
    {
        // 每字顯示間隔（秒）
        private const float CHAR_INTERVAL = 0.05f;

        private Coroutine _typingCoroutine;
        private bool _isTyping;
        private string _fullText;

        /// <summary>打字機完成或被中斷（顯示全文）後呼叫。</summary>
        public Action<string> OnTextUpdated;  // 目前應顯示的文字
        public Action OnTypingComplete;       // 打字機自然結束（全文顯示完畢）

        /// <summary>開始播放一句話。</summary>
        public void Play(string text)
        {
            _fullText = text;

            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);

            _typingCoroutine = StartCoroutine(TypingRoutine(text));
        }

        /// <summary>
        /// 玩家點擊時呼叫。
        /// 打字機進行中 → 立即顯示全文並停止打字機（不觸發 OnTypingComplete，由外部推進邏輯處理）。
        /// 打字機已結束 → 不在此處理，由 DialogueFlowController.Advance() 推進。
        /// </summary>
        /// <returns>true = 已中斷打字機；false = 打字機已結束，沒有中斷。</returns>
        public bool TryInterrupt()
        {
            if (!_isTyping) return false;

            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);

            _isTyping = false;
            OnTextUpdated?.Invoke(_fullText);
            return true;
        }

        public bool IsTyping => _isTyping;

        private IEnumerator TypingRoutine(string text)
        {
            _isTyping = true;
            var sb = new System.Text.StringBuilder();

            foreach (char c in text)
            {
                sb.Append(c);
                OnTextUpdated?.Invoke(sb.ToString());
                yield return new WaitForSeconds(CHAR_INTERVAL);
            }

            _isTyping = false;
            OnTypingComplete?.Invoke();
        }
    }
}
