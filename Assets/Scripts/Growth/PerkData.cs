namespace Celea
{
    public enum PerkPoolType
    {
        Virtue,
        Neutral,
        Sin
    }

    [System.Serializable]
    public class PerkData
    {
        public string perkId;
        public string displayName;
        public PerkPoolType pool;
        public string effectType;  // 佔位：後期替換為具體 enum 或效果物件
        public float effectValue;  // 佔位：待設計端定義後填入
    }
}
