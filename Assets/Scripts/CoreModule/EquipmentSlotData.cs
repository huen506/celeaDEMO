using System.Collections.Generic;

namespace Celea
{
    [System.Serializable]
    public class EquipmentSlotData
    {
        // 佔位：從 2 洞開始，每兩章 +1，上限 6，待設計端定義後填入
        public int unlockedSlots = 2;
        public const int MAX_SLOTS = 6;

        public List<CoreModuleData> equippedModules = new List<CoreModuleData>();

        public bool CanEquip => equippedModules.Count < unlockedSlots;

        public bool TryEquip(CoreModuleData module)
        {
            if (!CanEquip) return false;
            equippedModules.Add(module);
            return true;
        }

        public bool Unequip(string moduleId)
        {
            var mod = equippedModules.Find(m => m.moduleId == moduleId);
            if (mod == null) return false;
            // 紅色魔核拆除後報廢
            equippedModules.Remove(mod);
            return true;
        }
    }
}
