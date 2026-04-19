using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 單一場景節點的資料結構。
    /// 記錄節點類型、連結關係與解鎖狀態。
    /// </summary>
    [System.Serializable]
    public class SceneNode
    {
        public enum NodeType
        {
            Normal,     // 一般場景（城鎮、建築內部）
            Exploration // 探索場景（野外箱庭）
        }

        [SerializeField] private string _nodeId;
        [SerializeField] private string _displayName;
        [SerializeField] private NodeType _nodeType;
        [SerializeField] private List<string> _connectedNodeIds = new List<string>();

        private bool _isUnlocked;

        public string NodeId       => _nodeId;
        public string DisplayName  => _displayName;
        public NodeType Type       => _nodeType;
        public bool IsUnlocked     => _isUnlocked;
        public IReadOnlyList<string> ConnectedNodeIds => _connectedNodeIds;

        public SceneNode(string nodeId, string displayName, NodeType nodeType)
        {
            _nodeId      = nodeId;
            _displayName = displayName;
            _nodeType    = nodeType;
            _isUnlocked  = false;
        }

        /// <summary>
        /// 解鎖節點。玩家第一次實際踏入時自動呼叫。
        /// </summary>
        public void Unlock()
        {
            _isUnlocked = true;
        }

        /// <summary>
        /// 新增連結節點 ID（建立出入口關係）。
        /// </summary>
        public void AddConnection(string targetNodeId)
        {
            if (!_connectedNodeIds.Contains(targetNodeId))
                _connectedNodeIds.Add(targetNodeId);
        }
    }
}
