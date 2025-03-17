using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class BattleEntity : MonoBehaviour, IBattleEntity
{
    [SerializeField] private IBattleEntity.Type _type;

    private Animator _anim;

    [SerializeField] private EntityStats _stats;

    private Stats _currentStats;
    public Stats CurrentStats => _currentStats;

    private void Awake()
    {
        _anim = GetComponent<Animator>();

        _currentStats = _stats.Stats;
    }

    public string GetName() => name;

    public IBattleEntity.Type GetEntityType() => _type;
    public List<Card> GetCards() => _currentStats.Cards;

    public abstract IEnumerator ChooseAttack(IEnumerable<Card> options);
    public abstract IEnumerator ChooseTarget(IEnumerable<IBattleEntity> options);

    public virtual void Attack(AttackResults attack, IBattleEntity target)
    {
        attack.damage /= _currentStats.Defense;
        attack.result = _currentStats.ModifyHealth(attack.damage);

        if (attack.result == AttackResults.HealthChangeResult.Dead)
            attack.defender.Die();
    }

    public void Die()
    {
        _anim.SetTrigger("Die");
    }
}
