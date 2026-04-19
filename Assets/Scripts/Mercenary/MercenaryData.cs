using System.Collections.Generic;

namespace Celea
{
    [System.Serializable]
    public class MercenaryData
    {
        public string mercenaryId;
        public string displayName;
        public List<string> preferredMoralTiers = new List<string>();  // 可接受的光譜範圍（Virtue/Neutral/Sin）
        public bool isAlive = true;
        public string routeExclusive;  // 空=共用池；Tech/Nature/Neutral=線路專屬

        // 傭兵個別能力值固定（佔位，待設計端定義）
        public float baseAttack = 10f;
        public float baseDefense = 5f;
        public float baseHp = 100f;
    }
}
