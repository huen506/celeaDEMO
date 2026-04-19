using System.Collections.Generic;

namespace Celea
{
    /// <summary>
    /// 整份對話資料的容器。對應一份 JSON 檔案。
    /// 使用陣列格式讓 JsonUtility 可以直接解析，不需要第三方套件。
    /// </summary>
    [System.Serializable]
    public class DialogueData
    {
        /// <summary>這份對話的唯一識別碼，對應 JSON 檔案名稱（例：D001）。</summary>
        public string dialogueId;

        /// <summary>對話從哪一段開始播。</summary>
        public string startSegmentId;

        /// <summary>所有段落的陣列，用迴圈搜尋 segmentId 找到目標段落。</summary>
        public List<DialogueSegment> segments;

        /// <summary>以 segmentId 搜尋並回傳對應段落，找不到回傳 null。</summary>
        public DialogueSegment FindSegment(string segmentId)
        {
            if (segments == null) return null;
            foreach (var seg in segments)
            {
                if (seg.segmentId == segmentId) return seg;
            }
            return null;
        }
    }
}
