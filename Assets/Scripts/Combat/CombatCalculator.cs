namespace Celea
{
    public class CombatCalculator
    {
        private BattleStatsBuilder statsBuilder;

        public CombatCalculator(BattleStatsBuilder builder)
        {
            statsBuilder = builder;
        }

        public float CalculateDamage(CombatUnit attacker, CombatUnit target, ActionEffect effect)
        {
            var attackerStats = statsBuilder.GetStats(attacker.unitId);
            var targetStats = statsBuilder.GetStats(target.unitId);

            float baseDamage = effect.value + attackerStats.attack - targetStats.defense;
            if (baseDamage < 1f) baseDamage = 1f;
            return baseDamage;
        }

        public float CalculateHeal(CombatUnit source, ActionEffect effect)
        {
            return effect.value;
        }
    }
}
