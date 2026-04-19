namespace Celea
{
    public enum LogEntryType { Normal, InnerThought, Choice }

    [System.Serializable]
    public class DialogueLogEntry
    {
        public string       entryId;
        public string       speakerId;
        public string       speakerDisplayName;
        public string       text;
        public LogEntryType entryType;
        public bool         isMarked;
        public bool         hasImplicitConsequence;
        public string       timestamp;
    }
}
