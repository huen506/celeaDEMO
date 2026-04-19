using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    [System.Serializable]
    public class PerkDatabaseJson
    {
        public List<PerkData> perks = new List<PerkData>();
    }

    public class PerkDatabase
    {
        private List<PerkData> virtuePool = new List<PerkData>();
        private List<PerkData> neutralPool = new List<PerkData>();
        private List<PerkData> sinPool = new List<PerkData>();

        public void Load()
        {
            virtuePool  = LoadPool("Perks/PerkDatabase_Virtue");
            neutralPool = LoadPool("Perks/PerkDatabase_Neutral");
            sinPool     = LoadPool("Perks/PerkDatabase_Sin");
        }

        private List<PerkData> LoadPool(string resourcePath)
        {
            var asset = Resources.Load<TextAsset>(resourcePath);
            if (asset == null)
            {
                Debug.LogWarning($"[PerkDatabase] 找不到資料檔：{resourcePath}");
                return new List<PerkData>();
            }
            var db = JsonUtility.FromJson<PerkDatabaseJson>(asset.text);
            return db?.perks ?? new List<PerkData>();
        }

        public List<PerkData> GetPool(PerkPoolType poolType)
        {
            return poolType switch
            {
                PerkPoolType.Virtue  => virtuePool,
                PerkPoolType.Neutral => neutralPool,
                PerkPoolType.Sin     => sinPool,
                _                   => neutralPool
            };
        }
    }
}
