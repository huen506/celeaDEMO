using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    [System.Serializable]
    public class MercenaryDatabaseJson
    {
        public List<MercenaryData> mercenaries = new List<MercenaryData>();
    }

    public class MercenaryDatabase
    {
        private List<MercenaryData> allMercenaries = new List<MercenaryData>();

        public void Load()
        {
            var asset = Resources.Load<TextAsset>("Mercenaries/MercenaryDatabase");
            if (asset == null)
            {
                Debug.LogWarning("[MercenaryDatabase] 找不到資料檔：Mercenaries/MercenaryDatabase");
                return;
            }
            var db = JsonUtility.FromJson<MercenaryDatabaseJson>(asset.text);
            allMercenaries = db?.mercenaries ?? new List<MercenaryData>();
        }

        public List<MercenaryData> GetAll() => new List<MercenaryData>(allMercenaries);

        public MercenaryData FindById(string id) => allMercenaries.Find(m => m.mercenaryId == id);
    }
}
