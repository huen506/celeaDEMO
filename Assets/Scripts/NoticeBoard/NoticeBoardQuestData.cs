using System.Collections.Generic;

namespace Celea
{
    public enum QuestState  { Unavailable, Available, Active, Completed, Failed }
    public enum StepState   { Locked, Revealed, Completed }

    [System.Serializable]
    public class QuestStep
    {
        public int       stepIndex;
        public string    description;
        public string    completionCondition;
        public StepState stepState;
    }

    [System.Serializable]
    public class NoticeBoardQuestData
    {
        public string           questId;
        public string           townId;
        public string           title;
        public List<QuestStep>  steps;
        public int              timeLimitDays;
        public QuestState       questState;
        public int              currentStepIndex;
        public int              acceptedDay;
        public bool             isMarked;
        public string           consequenceKey;
    }

    [System.Serializable]
    public class TownQuestCollection
    {
        public string                   townId;
        public List<NoticeBoardQuestData> quests;
    }
}
