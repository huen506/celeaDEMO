namespace Celea
{
    public enum ResonanceState
    {
        Normal,
        Resonating
    }

    // 崩鳴的唯一資料來源，CombatUnit 只持有此類別的唯讀引用
    public class ResonanceData
    {
        public string unitId { get; private set; }
        public float currentValue { get; private set; }
        public float maxValue => ResonanceConfig.RESONANCE_MAX;
        public ResonanceState state { get; private set; }
        public int resonancePoints { get; private set; }

        public bool IsResonating => state == ResonanceState.Resonating;

        public ResonanceData(string id)
        {
            unitId = id;
            // 每場戰鬥一律從 0 開始，不繼承前一場殘值
            currentValue = 0f;
            state = ResonanceState.Normal;
            resonancePoints = 0;
        }

        internal void AddValue(float amount)
        {
            if (state == ResonanceState.Resonating) return;
            currentValue += amount;
            if (currentValue >= maxValue)
            {
                currentValue = maxValue;
                EnterResonance();
            }
        }

        private void EnterResonance()
        {
            state = ResonanceState.Resonating;
            resonancePoints = (int)maxValue;
        }

        internal void ConsumePoints(int amount)
        {
            if (state != ResonanceState.Resonating) return;
            resonancePoints -= amount;
            if (resonancePoints <= 0)
            {
                resonancePoints = 0;
                ExitResonance();
            }
        }

        private void ExitResonance()
        {
            state = ResonanceState.Normal;
            currentValue = 0f;
        }
    }
}
