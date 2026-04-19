namespace Celea
{
    public enum ModuleColor
    {
        Red,
        Blue,
        Green
    }

    [System.Serializable]
    public class CoreModuleData
    {
        public string moduleId;
        public ModuleColor color;
        public string effectType;   // 佔位：後期替換為具體效果定義
        public float effectValue;   // 佔位：待設計端定義後填入

        // 紅色魔核後台記錄，UI 層絕對不呈現（無公開 getter，外部無法直接讀取）
        [UnityEngine.SerializeField] private int _usageCount;
        public int maxUsage;        // 佔位：待設計端定義後填入

        public bool IsActive => color != ModuleColor.Red || _usageCount < maxUsage;

        // CoreModuleManager 專屬入口，外部類別不應直接呼叫
        internal void IncrementUsage() { _usageCount++; }
    }
}
