using UnityEngine;

namespace Celea
{
    /// <summary>
    /// 善惡光譜。持有光譜數值、判斷當前階段、提供選項範圍。
    /// 階段門檻待設計端定義後填入常數，邏輯結構不變。
    /// </summary>
    public class MoralSpectrum
    {
        // 佔位常數：待設計端定義後替換，不改邏輯結構
        private const int TIER_VIRTUE_2_THRESHOLD  =  60;
        private const int TIER_VIRTUE_1_THRESHOLD  =  25;
        private const int TIER_NEUTRAL_MAX         =  24;
        private const int TIER_NEUTRAL_MIN         = -25;
        private const int TIER_SIN_1_THRESHOLD     = -25;
        private const int TIER_SIN_2_THRESHOLD     = -60;

        // 五個階段：-2 到 +2
        public const int TIER_SIN_2    = -2;
        public const int TIER_SIN_1    = -1;
        public const int TIER_NEUTRAL  =  0;
        public const int TIER_VIRTUE_1 =  1;
        public const int TIER_VIRTUE_2 =  2;

        private int _value;

        public int Value => _value;

        /// <summary>
        /// 根據選擇自白的 payloadKey 調整光譜數值。
        /// 消極選項（Neutral）在善第二階段會降低光譜，在惡第二階段會提升光譜。
        /// </summary>
        public void ApplyChoice(string payloadKey, ChoiceCategory category)
        {
            int delta = GetDeltaForPayload(payloadKey, category);
            _value += delta;
            Debug.Log($"[MoralSpectrum] {payloadKey} → delta {delta}，新數值 {_value}，階段 {GetTier()}");
        }

        /// <summary>回傳當前善惡光譜階段（-2 到 +2）。</summary>
        public int GetTier()
        {
            if (_value >= TIER_VIRTUE_2_THRESHOLD)  return TIER_VIRTUE_2;
            if (_value >= TIER_VIRTUE_1_THRESHOLD)  return TIER_VIRTUE_1;
            if (_value > TIER_NEUTRAL_MIN)          return TIER_NEUTRAL;
            if (_value > TIER_SIN_2_THRESHOLD)      return TIER_SIN_1;
            return TIER_SIN_2;
        }

        /// <summary>打包存檔資料。</summary>
        public MoralSpectrumSaveData CaptureState()
        {
            return new MoralSpectrumSaveData { value = _value };
        }

        /// <summary>從存檔資料還原。</summary>
        public void RestoreState(MoralSpectrumSaveData saveData)
        {
            _value = saveData.value;
        }

        private int GetDeltaForPayload(string payloadKey, ChoiceCategory category)
        {
            // 消極選項的特殊邏輯：善第二降低光譜，惡第二提升光譜
            if (category == ChoiceCategory.Neutral)
            {
                int tier = GetTier();
                if (tier == TIER_VIRTUE_2) return -10; // 佔位數值，待設計端定義
                if (tier == TIER_SIN_2)   return  10; // 佔位數值，待設計端定義
                return 0;
            }

            // 美德正向，原罪負向
            // 佔位數值：待設計端定義 payloadKey 對應的具體數值後填入
            if (category == ChoiceCategory.Virtue) return 10;
            if (category == ChoiceCategory.Sin)    return -10;
            return 0;
        }
    }

    [System.Serializable]
    public class MoralSpectrumSaveData
    {
        public int value;
    }
}
