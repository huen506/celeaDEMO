using System.Collections.Generic;

namespace Celea
{
    [System.Serializable]
    public class TierQuestList
    {
        public int          tier;
        public List<string> questIds;
    }

    [System.Serializable]
    public class HiddenQuestPool
    {
        public string            characterId;
        public List<TierQuestList> questsByTier;
    }
}
