using System;

namespace Celea
{
    /// <summary>
    /// 存檔資料聚合格式。包含所有子系統的存檔資料。
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        /// <summary>存檔版本號，用於版本相容性判斷。</summary>
        public int saveVersion;

        /// <summary>存檔時間（ISO 8601 字串，JsonUtility 序列化用）。</summary>
        public string saveTimestamp;

        /// <summary>旗標系統存檔資料。</summary>
        public FlagSaveData flagData;

        /// <summary>時間系統存檔資料。</summary>
        public TimeSaveData timeData;

        /// <summary>場景系統存檔資料。</summary>
        public SceneSaveData sceneData;

        /// <summary>對話履歷存檔資料。</summary>
        public LogSaveData logData;

        /// <summary>隱性任務存檔資料。</summary>
        public HiddenQuestSaveData hiddenQuestData;

        /// <summary>佈告欄任務存檔資料。</summary>
        public NoticeBoardSaveData noticeBoardData;

        /// <summary>圖鑑存檔資料。</summary>
        public GallerySaveData galleryData;

        // 第四批新增欄位（saveVersion 遞增後，舊存檔清除不轉換）

        /// <summary>傷勢系統存檔資料（跨場景累積）。</summary>
        public InjurySaveData injuryData;

        /// <summary>養成系統存檔資料（已獲加護技能與好感度解鎖）。</summary>
        public GrowthSaveData growthData;

        /// <summary>魔核系統存檔資料（科技線）。</summary>
        public CoreModuleSaveData coreModuleData;

        /// <summary>傭兵池存檔資料（存活與傷勢狀態）。</summary>
        public MercenarySaveData mercenaryData;
    }

    // InjuryManager.CaptureState() 的序列化包裝（Dictionary 不可直接序列化）
    [System.Serializable]
    public class InjurySaveData
    {
        public System.Collections.Generic.List<InjuryData> entries = new System.Collections.Generic.List<InjuryData>();
    }
}
