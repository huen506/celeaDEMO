namespace Celea
{
    /// <summary>
    /// 一句話的完整資料格式。
    /// </summary>
    [System.Serializable]
    public class DialogueLine
    {
        /// <summary>說話者的 ID，用來對應立繪與姓名。</summary>
        public string speakerId;

        /// <summary>顯示在對話框上的說話者名稱。</summary>
        public string speakerDisplayName;

        /// <summary>對話文字內容。</summary>
        public string text;

        /// <summary>配音資源的索引鍵，無配音時留空。</summary>
        public string voiceClipKey;

        /// <summary>true = 內心獨白（OS），場景調暗處理。</summary>
        public bool isInnerThought;

        /// <summary>說話者立繪的位置索引（0～3），決定哪個位置亮起。</summary>
        public int portraitFocusSlot;
    }
}
