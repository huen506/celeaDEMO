using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    [System.Serializable]
    public class GallerySaveData
    {
        public List<string> unlockedEntryIds = new List<string>();
    }

    public class GalleryManager : MonoBehaviour
    {
        private GalleryDatabase _database;

        private void Awake()
        {
            LoadDatabase();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_MAIN_EVENT, OnMainEvent);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_MAIN_EVENT, OnMainEvent);
        }

        // ── 資料載入 ─────────────────────────────────────────────

        private void LoadDatabase()
        {
            var asset = Resources.Load<TextAsset>("Gallery/GalleryDatabase");
            if (asset == null)
            {
                Debug.LogWarning("[GalleryManager] 找不到 GalleryDatabase.json。");
                return;
            }
            _database = JsonUtility.FromJson<GalleryDatabase>(asset.text);
        }

        // ── 事件接收 ─────────────────────────────────────────────

        private void OnMainEvent(EventData data)
        {
            string eventId = data.Get<string>("eventId");
            if (string.IsNullOrEmpty(eventId) || _database?.entries == null) return;

            foreach (var entry in _database.entries)
            {
                if (entry.unlockEventId != eventId) continue;
                if (entry.isUnlocked)               continue;

                entry.isUnlocked = true;

                var evt = new EventData();
                evt.Set("entryId", entry.entryId);
                EventManager.Instance.Publish(GameEvents.ON_GALLERY_UNLOCKED, evt);
            }
        }

        // ── 對外接口 ─────────────────────────────────────────────

        public List<GalleryEntryData> GetAllEntries()
        {
            var list = new List<GalleryEntryData>();
            if (_database?.entries != null)
                list.AddRange(_database.entries);
            list.Sort((a, b) => a.displayOrder.CompareTo(b.displayOrder));
            return list;
        }

        public List<GalleryEntryData> GetUnlockedEntries() =>
            GetAllEntries().FindAll(e => e.isUnlocked);

        public GallerySaveData CaptureState()
        {
            var save = new GallerySaveData();
            if (_database?.entries != null)
                foreach (var e in _database.entries)
                    if (e.isUnlocked) save.unlockedEntryIds.Add(e.entryId);
            return save;
        }

        public void RestoreState(GallerySaveData saveData)
        {
            if (_database?.entries == null || saveData?.unlockedEntryIds == null) return;
            foreach (var e in _database.entries)
                e.isUnlocked = saveData.unlockedEntryIds.Contains(e.entryId);
        }
    }
}
