using System;
using System.IO;
using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 儲存系統主系統。
    /// 監聽存檔事件、呼叫各系統 CaptureState / RestoreState、寫入讀取本地檔案。
    /// 不直接碰各系統的內部變數，只透過接口存取。
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        // 存檔版本號。格式不相容時需要遞增，讓 Demo 階段提示玩家重置
        // v2：第四批新增 injuryData / growthData / coreModuleData / mercenaryData
        private const int CURRENT_SAVE_VERSION = 2;

        private static string SAVE_PATH =>
            Path.Combine(Application.persistentDataPath, "save.json");

        [Header("各系統參考（在 Inspector 指定）")]
        [SerializeField] private FlagManager        _flagManager;
        [SerializeField] private TimeManager        _timeManager;
        [SerializeField] private CeleaSceneManager  _sceneManager;
        [SerializeField] private HiddenQuestManager _hiddenQuestManager;
        [SerializeField] private NoticeBoardManager _noticeBoardManager;
        [SerializeField] private DialogueLogManager _dialogueLogManager;
        [SerializeField] private GalleryManager     _galleryManager;

        [Header("第四批系統參考（在 Inspector 指定）")]
        [SerializeField] private InjuryManager      _injuryManager;
        [SerializeField] private GrowthManager      _growthManager;
        [SerializeField] private CoreModuleManager  _coreModuleManager;
        [SerializeField] private MercenaryPool      _mercenaryPool;

        // 儲存目前的存檔是否可以執行（對話或探索場景中不可存檔）
        private bool _saveAllowed = true;

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_SAVE_REQUESTED,   OnSaveRequested);
            EventManager.Instance.Subscribe(GameEvents.ON_DIALOGUE_START,   OnDialogueStart);
            EventManager.Instance.Subscribe(GameEvents.ON_DIALOGUE_END,     OnDialogueEnd);
            EventManager.Instance.Subscribe(GameEvents.ON_SCENE_ENTER,      OnSceneEnter);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_SAVE_REQUESTED,  OnSaveRequested);
            EventManager.Instance.Unsubscribe(GameEvents.ON_DIALOGUE_START,  OnDialogueStart);
            EventManager.Instance.Unsubscribe(GameEvents.ON_DIALOGUE_END,    OnDialogueEnd);
            EventManager.Instance.Unsubscribe(GameEvents.ON_SCENE_ENTER,     OnSceneEnter);
        }

        // ── 存檔狀態追蹤 ─────────────────────────────────────────

        private void OnDialogueStart(EventData data) => _saveAllowed = false;
        private void OnDialogueEnd(EventData data)   => _saveAllowed = true;

        private void OnSceneEnter(EventData data)
        {
            // 進入探索場景禁止存檔，離開時（進入非探索場景）恢復
            bool isExploration = _sceneManager != null && _sceneManager.IsInExplorationScene;
            _saveAllowed = !isExploration;
        }

        // ── 事件接收 ─────────────────────────────────────────────

        private void OnSaveRequested(EventData data)
        {
            if (!_saveAllowed)
            {
                // 不可存檔狀態：UI 層已攔截，正常不應發生，靜默忽略
                Debug.Log("[SaveManager] 當前狀態不可存檔，忽略存檔請求。");
                return;
            }

            Save();
        }

        // ── 公開接口 ─────────────────────────────────────────────

        /// <summary>執行存檔。</summary>
        public void Save()
        {
            if (_flagManager == null || _timeManager == null || _sceneManager == null)
            {
                Debug.LogError("[SaveManager] 核心子系統（FlagManager / TimeManager / CeleaSceneManager）參考未指定，存檔中止。");
                return;
            }

            var saveData = new SaveData
            {
                saveVersion      = CURRENT_SAVE_VERSION,
                saveTimestamp    = DateTime.UtcNow.ToString("o"),
                flagData         = _flagManager.CaptureState(),
                timeData         = _timeManager.CaptureState(),
                sceneData        = _sceneManager.CaptureState(),
                hiddenQuestData  = _hiddenQuestManager != null ? _hiddenQuestManager.CaptureState()  : null,
                noticeBoardData  = _noticeBoardManager != null ? _noticeBoardManager.CaptureState()  : null,
                logData          = _dialogueLogManager != null ? _dialogueLogManager.CaptureState()  : null,
                galleryData      = _galleryManager     != null ? _galleryManager.CaptureState()      : null,
                injuryData       = _injuryManager      != null ? _injuryManager.CaptureState()       : null,
                growthData       = _growthManager      != null ? _growthManager.CaptureState()       : null,
                coreModuleData   = _coreModuleManager  != null ? _coreModuleManager.CaptureState()   : null,
                mercenaryData    = _mercenaryPool       != null ? _mercenaryPool.CaptureState()       : null,
            };

            string json = JsonUtility.ToJson(saveData, prettyPrint: true);

            // 先寫入暫存檔，成功後才替換正式存檔，避免寫入失敗時覆蓋舊存檔
            string tempPath = SAVE_PATH + ".tmp";
            try
            {
                File.WriteAllText(tempPath, json);
                File.Copy(tempPath, SAVE_PATH, overwrite: true);
                File.Delete(tempPath);
                Debug.Log($"[SaveManager] 存檔成功：{SAVE_PATH}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 存檔失敗：{e.Message}");
                // 保留上一份有效存檔，不覆蓋
                if (File.Exists(tempPath)) File.Delete(tempPath);
                // 待 UI 層接收此失敗訊號後向玩家顯示提示
            }
        }

        /// <summary>執行讀取。讀取失敗或版本不相容時清除舊存檔。</summary>
        public bool Load()
        {
            if (!File.Exists(SAVE_PATH))
            {
                Debug.Log("[SaveManager] 沒有存檔可以讀取。");
                return false;
            }

            try
            {
                string json     = File.ReadAllText(SAVE_PATH);
                var saveData    = JsonUtility.FromJson<SaveData>(json);

                if (saveData == null)
                    throw new Exception("存檔解析失敗，資料為 null。");

                // Demo 階段：版本不相容 → 清除舊存檔，提示重新開始
                if (saveData.saveVersion != CURRENT_SAVE_VERSION)
                {
                    Debug.LogWarning($"[SaveManager] 存檔版本不相容（存檔 v{saveData.saveVersion}，當前 v{CURRENT_SAVE_VERSION}），清除存檔。");
                    DeleteSave();
                    return false;
                }

                // 還原順序硬編碼，依照設計規格書第九節 9-2 固定順序，不可變更
                Debug.Log("[SaveManager][RestoreOrder] 1/11 Time");
                _timeManager.RestoreState(saveData.timeData);
                Debug.Log("[SaveManager][RestoreOrder] 2/11 Flag");
                _flagManager.RestoreState(saveData.flagData);
                if (_injuryManager      != null) { Debug.Log("[SaveManager][RestoreOrder] 3/11 Injury");     _injuryManager.RestoreState(saveData.injuryData);           }
                if (_growthManager      != null) { Debug.Log("[SaveManager][RestoreOrder] 4/11 Growth");     _growthManager.RestoreState(saveData.growthData);           }
                if (_coreModuleManager  != null) { Debug.Log("[SaveManager][RestoreOrder] 5/11 CoreModule"); _coreModuleManager.RestoreState(saveData.coreModuleData);   }
                if (_mercenaryPool      != null) { Debug.Log("[SaveManager][RestoreOrder] 6/11 Mercenary");  _mercenaryPool.RestoreState(saveData.mercenaryData);        }
                if (_hiddenQuestManager != null) { Debug.Log("[SaveManager][RestoreOrder] 7/11 HiddenQuest");_hiddenQuestManager.RestoreState(saveData.hiddenQuestData); }
                if (_noticeBoardManager != null) { Debug.Log("[SaveManager][RestoreOrder] 8/11 NoticeBoard");_noticeBoardManager.RestoreState(saveData.noticeBoardData); }
                if (_dialogueLogManager != null) { Debug.Log("[SaveManager][RestoreOrder] 9/11 DialogueLog");_dialogueLogManager.RestoreState(saveData.logData);         }
                if (_galleryManager     != null) { Debug.Log("[SaveManager][RestoreOrder] 10/11 Gallery");   _galleryManager.RestoreState(saveData.galleryData);         }
                Debug.Log("[SaveManager][RestoreOrder] 11/11 Scene");
                _sceneManager.RestoreState(saveData.sceneData);

                Debug.Log($"[SaveManager] 讀取成功（{saveData.saveTimestamp}）。");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 讀取存檔失敗：{e.Message}");
                return false;
            }
        }

        /// <summary>刪除存檔。</summary>
        public void DeleteSave()
        {
            if (File.Exists(SAVE_PATH))
            {
                File.Delete(SAVE_PATH);
                Debug.Log("[SaveManager] 存檔已刪除。");
            }
        }

        /// <summary>查詢是否存在存檔。</summary>
        public bool HasSave() => File.Exists(SAVE_PATH);

        /// <summary>查詢當前狀態是否允許存檔。</summary>
        public bool IsSaveAllowed => _saveAllowed;
    }
}
