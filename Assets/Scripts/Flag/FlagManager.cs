using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 旗標系統主系統。
    /// 接收事件並分派給子系統，提供對外統一查詢接口，自己不儲存資料。
    /// </summary>
    public class FlagManager : MonoBehaviour
    {
        private MoralSpectrum  _moralSpectrum  = new MoralSpectrum();
        private SituationFlags _situationFlags = new SituationFlags();
        private AffinitySystem _affinitySystem = new AffinitySystem();

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_CHOICE_MADE,  OnChoiceMade);
            EventManager.Instance.Subscribe(GameEvents.ON_CHAPTER_END,  OnChapterEnd);
            EventManager.Instance.Subscribe(GameEvents.ON_DAY_END,      OnDayEnd);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_CHOICE_MADE,  OnChoiceMade);
            EventManager.Instance.Unsubscribe(GameEvents.ON_CHAPTER_END,  OnChapterEnd);
            EventManager.Instance.Unsubscribe(GameEvents.ON_DAY_END,      OnDayEnd);
        }

        // ── 事件接收 ────────────────────────────────────────────

        private void OnChoiceMade(EventData data)
        {
            string categoryStr = data.Get<string>("choiceCategory");
            string payloadKey  = data.Get<string>("payloadKey");

            if (System.Enum.TryParse<ChoiceCategory>(categoryStr, out ChoiceCategory category))
                _moralSpectrum.ApplyChoice(payloadKey, category);
            else
                Debug.LogWarning($"[FlagManager] 無法解析 choiceCategory：{categoryStr}");
        }

        private void OnChapterEnd(EventData data)
        {
            _situationFlags.EvaluateChapterEnd();
        }

        private void OnDayEnd(EventData data)
        {
            bool rested = data.Get<bool>("rested");
            if (!rested)
            {
                // 不休息的好感度代價，具體扣減值待設計端定義，此處用佔位常數
                const int AFFINITY_PENALTY = -1;
                // 適用對象待設計端定義，此處預留接口
                Debug.Log($"[FlagManager] 玩家不休息，好感度扣減 {AFFINITY_PENALTY}（對象待定義）。");
            }
        }

        // ── 對外查詢接口 ─────────────────────────────────────────

        /// <summary>回傳當前善惡光譜階段（-2 到 +2）。</summary>
        public int GetMoralTier() => _moralSpectrum.GetTier();

        /// <summary>回傳善惡光譜的實際數值。</summary>
        public int GetMoralValue() => _moralSpectrum.Value;

        /// <summary>回傳指定角色的好感度數值（-3 到 +3）。</summary>
        public int GetAffinity(string characterId) => _affinitySystem.GetValue(characterId);

        /// <summary>回傳指定角色的好感度階段。</summary>
        public int GetAffinityTier(string characterId) => _affinitySystem.GetTier(characterId);

        /// <summary>回傳指定局勢旗標的累積數值。</summary>
        public int GetSituationValue(string flagId) => _situationFlags.GetValue(flagId);

        /// <summary>調整指定角色的好感度。由 HiddenQuestManager 等系統呼叫，不直接碰 AffinitySystem。</summary>
        public void ModifyAffinity(string characterId, int delta) =>
            _affinitySystem.AdjustAffinity(characterId, delta);

        // ── 存檔接口 ─────────────────────────────────────────────

        /// <summary>打包所有旗標資料供儲存系統使用。</summary>
        public FlagSaveData CaptureState()
        {
            return new FlagSaveData
            {
                moralData     = _moralSpectrum.CaptureState(),
                situationData = _situationFlags.CaptureState(),
                affinityData  = _affinitySystem.CaptureState()
            };
        }

        /// <summary>從儲存資料還原所有旗標狀態。</summary>
        public void RestoreState(FlagSaveData saveData)
        {
            _moralSpectrum.RestoreState(saveData.moralData);
            _situationFlags.RestoreState(saveData.situationData);
            _affinitySystem.RestoreState(saveData.affinityData);
        }
    }

    [System.Serializable]
    public class FlagSaveData
    {
        public MoralSpectrumSaveData  moralData;
        public SituationFlagsSaveData situationData;
        public AffinitySaveData       affinityData;
    }
}
