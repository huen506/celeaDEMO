using System.Collections.Generic;

namespace Celea
{
    [System.Serializable]
    public class AffinityUnlockData
    {
        public string characterId;
        public int tier;              // 1=熟識, 2=信任, 3=親密
        public List<string> unlockedAbilityIds = new List<string>();
        public float statBonus;       // 佔位：能力值提升數值，待設計端定義
        public bool enablesLinkSkill; // 親密時=true
    }
}
