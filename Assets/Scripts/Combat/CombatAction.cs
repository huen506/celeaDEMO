using System.Collections.Generic;

namespace Celea
{
    public enum ActionType
    {
        Attack,
        Skill,
        Ultimate,
        LinkSkill,
        SpecialAbility,
        Item,
        Guard
    }

    public enum EffectType
    {
        Damage,
        Heal,
        ApplyBuff,
        ApplyDebuff,
        ConsumeResonance
    }

    [System.Serializable]
    public class ActionEffect
    {
        public EffectType effectType;
        public float value;
        public string targetUnitId;
    }

    [System.Serializable]
    public class CombatAction
    {
        public ActionType actionType;
        public string sourceUnitId;
        public List<string> targetUnitIds = new List<string>();
        public string skillId;
        public List<ActionEffect> effects = new List<ActionEffect>();
        public Dictionary<string, object> metadata = new Dictionary<string, object>();
    }
}
