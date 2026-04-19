using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 玩家移動控制。
    /// 每次從一個節點移動到相鄰節點時，發出 ON_PLAYER_STEP 事件。
    /// 城鎮節點之間移動不消耗時段；探索場景移動透過 StepCounter 累積後通知 TimeManager。
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private string _currentNodeId;

        /// <summary>
        /// 移動到指定節點。由外部（UI、尋路系統）呼叫。
        /// </summary>
        public void MoveTo(string targetNodeId, SceneNode.NodeType nodeType)
        {
            if (_currentNodeId == targetNodeId) return;

            _currentNodeId = targetNodeId;

            // 探索場景移動才計步
            if (nodeType == SceneNode.NodeType.Exploration)
            {
                EventManager.Instance.Publish(GameEvents.ON_PLAYER_STEP);
            }
        }

        public string CurrentNodeId => _currentNodeId;
    }
}
