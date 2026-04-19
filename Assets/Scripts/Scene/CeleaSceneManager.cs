using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Celea
{
    /// <summary>
    /// Celēa 場景管理主系統。
    /// 命名為 CeleaSceneManager 以避免與 Unity 內建 SceneManager 衝突。
    /// 管理節點切換、解鎖狀態、場景狀態記憶、快速移動邏輯。
    /// </summary>
    public class CeleaSceneManager : MonoBehaviour
    {
        private static CeleaSceneManager _instance;
        public static CeleaSceneManager Instance => _instance;

        // 所有場景節點（nodeId → SceneNode）
        private readonly Dictionary<string, SceneNode> _nodes = new Dictionary<string, SceneNode>();

        // 場景狀態記憶（nodeId → SceneStateData）
        private readonly Dictionary<string, SceneStateData> _sceneStates = new Dictionary<string, SceneStateData>();

        private string _currentNodeId;
        private bool _isInExplorationScene;

        public string CurrentNodeId => _currentNodeId;
        public bool IsInExplorationScene => _isInExplorationScene;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ─────────────────────────────────────────
        // 節點管理
        // ─────────────────────────────────────────

        /// <summary>
        /// 登錄場景節點。遊戲初始化時將所有節點資料注入。
        /// </summary>
        public void RegisterNode(SceneNode node)
        {
            if (!_nodes.ContainsKey(node.NodeId))
                _nodes[node.NodeId] = node;
        }

        public SceneNode GetNode(string nodeId)
        {
            _nodes.TryGetValue(nodeId, out SceneNode node);
            return node;
        }

        // ─────────────────────────────────────────
        // 場景切換
        // ─────────────────────────────────────────

        /// <summary>
        /// 實際進入指定節點。
        /// 儲存當前場景狀態 → 淡入淡出佔位 → 載入新場景 → 還原狀態 → 發出事件。
        /// </summary>
        public void EnterNode(string targetNodeId)
        {
            SceneNode targetNode = GetNode(targetNodeId);
            if (targetNode == null)
            {
                Debug.LogWarning($"[CeleaSceneManager] 找不到節點：{targetNodeId}");
                return;
            }

            StartCoroutine(TransitionToNode(targetNode));
        }

        private IEnumerator TransitionToNode(SceneNode targetNode)
        {
            // 儲存當前場景狀態
            if (!string.IsNullOrEmpty(_currentNodeId))
                SaveCurrentSceneState();

            // 過場動畫佔位（淡入淡出，細節待美術端定義）
            yield return null;

            // 解鎖節點（玩家第一次踏入時自動解鎖）
            if (!targetNode.IsUnlocked)
                targetNode.Unlock();

            // 更新當前節點狀態
            _currentNodeId = targetNode.NodeId;
            _isInExplorationScene = targetNode.Type == SceneNode.NodeType.Exploration;

            // 載入 Unity 場景（場景名稱與 nodeId 對應，細節待場景建立後確認）
            // SceneManager.LoadScene(targetNode.NodeId); // 待場景建立後啟用

            // 還原場景狀態
            RestoreSceneState(_currentNodeId);

            // 發出場景進入事件
            EventData data = new EventData();
            data.Set("nodeId", _currentNodeId);
            data.Set("displayName", targetNode.DisplayName);
            EventManager.Instance.Publish(GameEvents.ON_SCENE_ENTER, data);
        }

        // ─────────────────────────────────────────
        // 快速移動
        // ─────────────────────────────────────────

        /// <summary>
        /// 嘗試快速移動到目標節點。
        /// 若當前在探索場景中，功能關閉。
        /// 目標節點未解鎖，拒絕並給予提示（提示文案待敘事端定義）。
        /// </summary>
        public bool TryFastTravel(string targetNodeId)
        {
            // 規則一：探索場景中，快速移動功能關閉
            if (_isInExplorationScene)
            {
                Debug.Log("[CeleaSceneManager] 探索場景中，快速移動不可用。");
                return false;
            }

            SceneNode targetNode = GetNode(targetNodeId);
            if (targetNode == null) return false;

            // 規則二：目標節點未解鎖，拒絕
            if (!targetNode.IsUnlocked)
            {
                Debug.Log($"[CeleaSceneManager] 節點 {targetNodeId} 尚未解鎖，無法快速移動。");
                // TODO: 顯示提示文案（待敘事端定義）
                return false;
            }

            // 允許快速移動
            EnterNode(targetNodeId);
            return true;
        }

        // ─────────────────────────────────────────
        // 場景狀態記憶
        // ─────────────────────────────────────────

        private void SaveCurrentSceneState()
        {
            if (string.IsNullOrEmpty(_currentNodeId)) return;

            if (!_sceneStates.ContainsKey(_currentNodeId))
                _sceneStates[_currentNodeId] = new SceneStateData { SceneNodeId = _currentNodeId };

            // 實際狀態收集由各子系統主動呼叫 GetOrCreateStateData 後寫入
        }

        private void RestoreSceneState(string nodeId)
        {
            if (!_sceneStates.TryGetValue(nodeId, out SceneStateData state))
            {
                // 無記憶資料 → 使用初始狀態，不中斷遊戲，不報錯
                return;
            }

            // 實際還原由各子系統主動讀取 GetStateData 後自行處理
        }

        /// <summary>
        /// 取得或建立指定節點的狀態資料。供子系統寫入使用。
        /// </summary>
        public SceneStateData GetOrCreateStateData(string nodeId)
        {
            if (!_sceneStates.TryGetValue(nodeId, out SceneStateData state))
            {
                state = new SceneStateData { SceneNodeId = nodeId };
                _sceneStates[nodeId] = state;
            }
            return state;
        }

        /// <summary>
        /// 讀取指定節點的狀態資料。若不存在回傳 null，子系統使用初始值。
        /// </summary>
        public SceneStateData GetStateData(string nodeId)
        {
            _sceneStates.TryGetValue(nodeId, out SceneStateData state);
            return state;
        }

        // ─────────────────────────────────────────
        // 場景解鎖條件接口（預留給敘事端使用）
        // ─────────────────────────────────────────

        /// <summary>
        /// 強制解鎖指定節點（由敘事系統在特定劇情條件達成後呼叫）。
        /// </summary>
        public void ForceUnlockNode(string nodeId)
        {
            SceneNode node = GetNode(nodeId);
            if (node != null && !node.IsUnlocked)
                node.Unlock();
        }

        // ─────────────────────────────────────────
        // 存檔接口
        // ─────────────────────────────────────────

        /// <summary>打包場景狀態供儲存系統使用。</summary>
        public SceneSaveData CaptureState()
        {
            var save = new SceneSaveData();
            save.currentNodeId = _currentNodeId;

            // 打包所有已解鎖的節點 ID
            save.unlockedNodeIds = new System.Collections.Generic.List<string>();
            foreach (var kv in _nodes)
            {
                if (kv.Value.IsUnlocked)
                    save.unlockedNodeIds.Add(kv.Key);
            }

            return save;
        }

        /// <summary>從存檔資料還原場景狀態。</summary>
        public void RestoreState(SceneSaveData saveData)
        {
            _currentNodeId = saveData.currentNodeId;

            if (saveData.unlockedNodeIds != null)
            {
                foreach (string nodeId in saveData.unlockedNodeIds)
                {
                    if (_nodes.TryGetValue(nodeId, out SceneNode node))
                        node.Unlock();
                }
            }
        }
    }

    [System.Serializable]
    public class SceneSaveData
    {
        public string currentNodeId;
        public System.Collections.Generic.List<string> unlockedNodeIds;
    }
}
