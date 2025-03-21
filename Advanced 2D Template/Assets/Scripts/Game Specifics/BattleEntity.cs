using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public abstract class BattleEntity : Selectable, IBattleEntity
{
    [SerializeField] private IBattleEntity.Type _type;

    private Animator _anim;

    [SerializeField] private EntityStats _stats;

    private Stats _currentStats;
    public virtual ref Stats GetStats() => ref _currentStats;
    public void SetStats(Stats stats) => _currentStats = stats;

    protected override void Awake()
    {
        _anim = GetComponent<Animator>();

        _currentStats = _stats.Stats;
    }

    public string GetName() => name;

    public IBattleEntity.Type GetEntityType() => _type;
    public List<Card> GetCards() => _currentStats.Cards;

    public abstract void InitAttack(IEnumerable<Card> options);
    public abstract (CustomYieldInstruction, Func<Card>) ChooseAttack(IEnumerable<Card> options);

    public abstract void InitTarget(IEnumerable<IBattleEntity> options);
    public abstract (CustomYieldInstruction, Func<IBattleEntity>) ChooseTarget(IEnumerable<IBattleEntity> options);

    public virtual AttackInfo Attack(Card attack, IBattleEntity target)
    {
        return new AttackInfo()
        {
            attacker = this,
            defender = target,
            card = attack
        };
    }

    public virtual void ReceiveAttack(AttackInfo info)
    {
        info.card.Effect.Invoke(info);
    }

    public void Die()
    {
        _anim.SetTrigger("Die");
    }
}
