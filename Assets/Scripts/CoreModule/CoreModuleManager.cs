using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    [System.Serializable]
    public class CoreModuleSaveData
    {
        public EquipmentSlotData slotData = new EquipmentSlotData();
    }

    // 全域持久，Singleton + DontDestroyOnLoad（科技線才掛載）
    public class CoreModuleManager : MonoBehaviour
    {
        public static CoreModuleManager Instance { get; private set; }

        private EquipmentSlotData slotData = new EquipmentSlotData();
        private bool isTechRoute = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // 確認當前線路，非科技線不初始化
            isTechRoute = CheckIsTechRoute();
            if (!isTechRoute)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private bool CheckIsTechRoute()
        {
            var flagManager = UnityEngine.Object.FindAnyObjectByType<FlagManager>();
            if (flagManager == null) return true; // 無旗標系統時預設科技線（開發期）
            return flagManager.GetSituationValue("route_tech") > 0;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // BattleStatsBuilder 查詢接口
        public List<object> GetActiveModuleEffects()
        {
            if (!isTechRoute) return new List<object>();

            var effects = new List<object>();
            foreach (var mod in slotData.equippedModules)
                if (mod.IsActive)
                    effects.Add(mod);
            return effects;
        }

        public bool TryEquip(CoreModuleData module)
        {
            if (!slotData.CanEquip)
            {
                Debug.LogWarning("[CoreModuleManager] 插槽已滿，無法嵌入魔核。");
                return false;
            }
            return slotData.TryEquip(module);
        }

        public bool TryUnequip(string moduleId)
        {
            var mod = slotData.equippedModules.Find(m => m.moduleId == moduleId);
            if (mod != null && mod.color == ModuleColor.Red)
            {
                // 紅色魔核拆除後報廢
                slotData.equippedModules.Remove(mod);
                return true;
            }
            return slotData.Unequip(moduleId);
        }

        // 外部系統（戰鬥獎勵、商店）取得魔核
        public void AcquireModule(CoreModuleData module)
        {
            // 取得後放入可用庫存（佔位：後期實作庫存管理）
            Debug.Log($"[CoreModuleManager] 取得魔核：{module.moduleId}（{module.color}）");
        }

        public void ConsumeRedModuleUsage(string moduleId)
        {
            var mod = slotData.equippedModules.Find(m => m.moduleId == moduleId && m.color == ModuleColor.Red);
            if (mod == null) return;
            mod.IncrementUsage();
            // 次數後台記錄，絕對不對玩家顯示
        }

        // SaveSystem 接口
        public CoreModuleSaveData CaptureState()
        {
            return new CoreModuleSaveData { slotData = slotData };
        }

        public void RestoreState(CoreModuleSaveData saved)
        {
            slotData = saved.slotData ?? new EquipmentSlotData();
        }
    }
}
