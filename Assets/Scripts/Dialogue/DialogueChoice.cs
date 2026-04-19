namespace Celea
{
    /// <summary>
    /// 選擇自白的選項分類。
    /// </summary>
    public enum ChoiceCategory
    {
        Virtue,   // 美德
        Neutral,  // 消極
        Sin       // 原罪
    }

    /// <summary>
    /// 一個選項的資料格式。
    /// </summary>
    [System.Serializable]
    public class DialogueChoice
    {
        /// <summary>選項的唯一識別碼。</summary>
        public string choiceId;

        /// <summary>選項顯示文字。</summary>
        public string text;

        /// <summary>
        /// Virtue（美德）/ Neutral（消極）/ Sin（原罪）。
        /// ⚠️ JSON 中必須填整數：Virtue=0、Neutral=1、Sin=2。
        /// JsonUtility 不支援 enum 字串，填字串會 fallback 到 0（Virtue）。
        /// </summary>
        public ChoiceCategory choiceCategory;

        /// <summary>傳給旗標系統的資料鍵，旗標系統根據此鍵更新善惡光譜。</summary>
        public string payloadKey;
    }
}
