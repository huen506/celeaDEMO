using System.Collections.Generic;

namespace Celea
{
    /// <summary>
    /// 單一場景的狀態記憶資料格式。
    /// 玩家離開場景時存入，再次進入時還原。
    /// </summary>
    [System.Serializable]
    public class SceneStateData
    {
        public string SceneNodeId;

        // NPC 狀態：NPC ID → NPC 當前狀態快照
        public Dictionary<string, NpcState> NpcStates = new Dictionary<string, NpcState>();

        // 物件狀態：物件 ID → 是否已互動（撿取、破壞等）
        public Dictionary<string, bool> ObjectInteracted = new Dictionary<string, bool>();

        // 時間對應變化：時間段識別符 → 外觀狀態識別符（細節待時間系統定義）
        public Dictionary<string, string> TimeVariantStates = new Dictionary<string, string>();

        // 已觸發事件：事件 ID → true 表示已觸發，不重複觸發
        public HashSet<string> TriggeredEvents = new HashSet<string>();

        /// <summary>
        /// 記錄事件已觸發。
        /// </summary>
        public void MarkEventTriggered(string eventId)
        {
            TriggeredEvents.Add(eventId);
        }

        /// <summary>
        /// 查詢事件是否已觸發過。
        /// </summary>
        public bool IsEventTriggered(string eventId)
        {
            return TriggeredEvents.Contains(eventId);
        }
    }

    /// <summary>
    /// NPC 狀態快照。
    /// </summary>
    [System.Serializable]
    public class NpcState
    {
        public string NpcId;
        public bool HasSpoken;       // 是否已對話
        public int DialogueProgress; // 對話進度索引
        // 位置資訊待場景座標系統定義後補充
    }
}
