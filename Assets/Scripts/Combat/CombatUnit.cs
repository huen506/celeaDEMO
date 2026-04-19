namespace Celea
{
    public class CombatUnit
    {
        public string unitId;
        public string displayName;
        public float maxHp;
        public float currentHp;

        public bool canAct = true;
        public bool hasActedThisTurn = false;

        // 唯讀引用，資料主權在 InjuryManager / ResonanceManager
        public InjuryData injuryData { get; private set; }
        public ResonanceData resonanceData { get; private set; }

        public bool IsDefeated => currentHp <= 0f;

        public void Initialize(string id, string name, float hp, InjuryData injury, ResonanceData resonance)
        {
            unitId = id;
            displayName = name;
            maxHp = hp;
            currentHp = hp;
            injuryData = injury;
            resonanceData = resonance;
        }

        public void TakeDamage(float amount)
        {
            currentHp -= amount;
            if (currentHp < 0f) currentHp = 0f;
        }

        public void Heal(float amount)
        {
            currentHp += amount;
            if (currentHp > maxHp) currentHp = maxHp;
        }

        public void ResetTurnState()
        {
            canAct = true;
            hasActedThisTurn = false;
        }
    }
}
