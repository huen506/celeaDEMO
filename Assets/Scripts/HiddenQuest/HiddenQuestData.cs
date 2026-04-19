namespace Celea
{
    [System.Serializable]
    public class HiddenQuestData
    {
        public string questId;
        public string characterId;
        public int    affinityTierRequired;
        public string triggerDialogueId;
        public string completionCondition;
        public int    timeLimitDays;
        public int    successAffinityDelta;
        public int    failureAffinityDelta;
        public string rewardKey;
        public bool   isUsed;
    }

    [System.Serializable]
    public class HiddenQuestDatabase
    {
        public HiddenQuestData[] quests;
    }
}
