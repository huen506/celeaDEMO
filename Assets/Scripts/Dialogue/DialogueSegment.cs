using System.Collections.Generic;

namespace Celea
{
    /// <summary>
    /// 一段對話（多句話＋選擇自白）的資料格式。
    /// </summary>
    [System.Serializable]
    public class DialogueSegment
    {
        /// <summary>這段對話的唯一識別碼。</summary>
        public string segmentId;

        /// <summary>這段對話包含的所有句子，依序播出。</summary>
        public List<DialogueLine> lines;

        /// <summary>true = 播完後插入選擇自白。</summary>
        public bool hasChoice;

        /// <summary>選擇自白的三個選項（hasChoice 為 true 時使用）。</summary>
        public List<DialogueChoice> choices;

        /// <summary>選擇完或播完後接續的下一段 ID，空字串代表結束。</summary>
        public string nextSegmentId;
    }
}
