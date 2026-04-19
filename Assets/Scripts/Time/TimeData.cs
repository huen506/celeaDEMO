namespace Celea
{
    /// <summary>
    /// 時段列舉定義。
    /// </summary>
    public enum TimeSlot
    {
        Dawn,      // 清晨：一天開始，跨日後進入
        Noon,      // 正午
        Dusk,      // 黃昏
        Night,     // 夜晚：正常結束點，行動結束後觸發跨日提示
        Midnight   // 深夜：特殊時段，選擇不休息才進入
    }

    /// <summary>
    /// 時間系統存檔資料格式。
    /// </summary>
    [System.Serializable]
    public class TimeSaveData
    {
        public TimeSlot currentSlot;
        public int      dayCount;
        public int      consecutiveNoRestDays;
    }
}
