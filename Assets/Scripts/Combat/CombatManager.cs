using System.Collections.Generic;
using UnityEngine;

namespace Celea
{
    public enum CombatState
    {
        Idle,
        TurnStart,
        SelectAction,
        ExecuteAction,
        TurnEnd,
        CombatEnd
    }

    public enum CombatResult
    {
        None,
        Victory,
        Defeat
    }

    // 戰鬥場景 scoped，不 DontDestroyOnLoad
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance { get; private set; }

        private CombatState currentState = CombatState.Idle;
        private List<CombatUnit> allUnits = new List<CombatUnit>();
        private List<CombatUnit> playerTeam = new List<CombatUnit>();
        private List<CombatUnit> enemyTeam = new List<CombatUnit>();
        private int turnIndex = 0;

        private BattleStatsBuilder statsBuilder;
        private CombatCalculator calculator;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            statsBuilder = GetComponent<BattleStatsBuilder>();
            if (statsBuilder == null) statsBuilder = gameObject.AddComponent<BattleStatsBuilder>();
            calculator = new CombatCalculator(statsBuilder);
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvents.ON_COMBAT_START, OnCombatStart);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvents.ON_COMBAT_START, OnCombatStart);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void OnCombatStart(EventData data)
        {
            TransitionTo(CombatState.TurnStart);
        }

        private void TransitionTo(CombatState next)
        {
            currentState = next;
            switch (next)
            {
                case CombatState.TurnStart:
                    HandleTurnStart();
                    break;
                case CombatState.SelectAction:
                    HandleSelectAction();
                    break;
                case CombatState.TurnEnd:
                    HandleTurnEnd();
                    break;
                case CombatState.CombatEnd:
                    HandleCombatEnd();
                    break;
            }
        }

        private void HandleTurnStart()
        {
            ResetUnitTurnStates();
            EventManager.Instance.Publish(GameEvents.ON_TURN_START, new EventData());
            TransitionTo(CombatState.SelectAction);
        }

        private void HandleSelectAction()
        {
            // 等待玩家輸入，由 PlayerSelectAction() 推進
        }

        // 外部（UI）呼叫此方法提交指令
        public void PlayerSelectAction(CombatAction action)
        {
            if (currentState != CombatState.SelectAction) return;
            TransitionTo(CombatState.ExecuteAction);
            ExecuteAction(action);
        }

        private void ExecuteAction(CombatAction action)
        {
            // 允許子流程插入（反擊、追擊、被動響應等），不假設單一線性路徑
            ProcessActionEffects(action);
            EventManager.Instance.Publish(GameEvents.ON_ACTION_EXECUTED, new EventData());
            TransitionTo(CombatState.TurnEnd);
        }

        private void ProcessActionEffects(CombatAction action)
        {
            foreach (var effect in action.effects)
            {
                var target = FindUnit(effect.targetUnitId);
                if (target == null) continue;

                switch (effect.effectType)
                {
                    case EffectType.Damage:
                        var source = FindUnit(action.sourceUnitId);
                        if (source != null)
                        {
                            float dmg = calculator.CalculateDamage(source, target, effect);
                            target.TakeDamage(dmg);
                            ResonanceManager.Instance?.AddResonance(source.unitId, ResonanceGainType.DealDamage);
                            ResonanceManager.Instance?.AddResonance(target.unitId, ResonanceGainType.TakeHit);
                        }
                        break;
                    case EffectType.Heal:
                        var healer = FindUnit(action.sourceUnitId);
                        if (healer != null)
                        {
                            float heal = calculator.CalculateHeal(healer, effect);
                            target.Heal(heal);
                            ResonanceManager.Instance?.AddResonance(healer.unitId, ResonanceGainType.Support);
                        }
                        break;
                    case EffectType.ConsumeResonance:
                        ResonanceManager.Instance?.ConsumeResonancePoints(effect.targetUnitId, (int)effect.value);
                        break;
                }

                if (target.IsDefeated) HandleUnitDefeated(target);
            }
        }

        private void HandleUnitDefeated(CombatUnit unit)
        {
            var data = new EventData();
            data.Set("unitId", unit.unitId);
            data.Set("isKeyCharacter", IsKeyCharacter(unit.unitId));
            EventManager.Instance.Publish(GameEvents.ON_UNIT_DEFEATED, data);

            if (unit.unitId == "Draven")
            {
                TriggerCombatEnd(CombatResult.Defeat);
            }
        }

        private bool IsKeyCharacter(string unitId)
        {
            // 佔位：科技線=莉歐拉、自然線=西芙、中立線=米蕾爾
            return unitId == "Leora" || unitId == "Siv" || unitId == "Mirel";
        }

        private void HandleTurnEnd()
        {
            EventManager.Instance.Publish(GameEvents.ON_TURN_END, new EventData());

            if (CheckVictory())
            {
                TriggerCombatEnd(CombatResult.Victory);
            }
            else if (CheckDefeat())
            {
                TriggerCombatEnd(CombatResult.Defeat);
            }
            else
            {
                AdvanceTurn();
                TransitionTo(CombatState.TurnStart);
            }
        }

        private void TriggerCombatEnd(CombatResult result)
        {
            currentState = CombatState.CombatEnd;
            HandleCombatEnd(result);
        }

        private void HandleCombatEnd(CombatResult result = CombatResult.None)
        {
            var data = new EventData();
            data.Set("result", result.ToString());
            EventManager.Instance.Publish(GameEvents.ON_COMBAT_END, data);
            currentState = CombatState.Idle;
        }

        private bool CheckVictory()
        {
            foreach (var unit in enemyTeam)
                if (!unit.IsDefeated) return false;
            return enemyTeam.Count > 0;
        }

        private bool CheckDefeat()
        {
            foreach (var unit in playerTeam)
                if (!unit.IsDefeated) return false;
            return playerTeam.Count > 0;
        }

        private void AdvanceTurn()
        {
            turnIndex = (turnIndex + 1) % allUnits.Count;
        }

        private void ResetUnitTurnStates()
        {
            foreach (var unit in allUnits)
                unit.ResetTurnState();
        }

        private CombatUnit FindUnit(string unitId)
        {
            return allUnits.Find(u => u.unitId == unitId);
        }

        public void RegisterUnit(CombatUnit unit, bool isPlayer)
        {
            allUnits.Add(unit);
            if (isPlayer) playerTeam.Add(unit);
            else enemyTeam.Add(unit);
        }

        public CombatState GetCurrentState() => currentState;
    }
}
