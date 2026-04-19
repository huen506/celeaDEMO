namespace Celea
{
    [System.Serializable]
    public class GalleryEntryData
    {
        public string entryId;
        public string resourceKey;
        public string unlockEventId;
        public bool   isUnlocked;
        public int    displayOrder;
    }

    [System.Serializable]
    public class GalleryDatabase
    {
        public GalleryEntryData[] entries;
    }
}
