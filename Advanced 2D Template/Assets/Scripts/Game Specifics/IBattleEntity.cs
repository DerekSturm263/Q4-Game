using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleEntity
{
    public enum Type
    {
        Human = 1,
        AI = -1
    }

    public string GetName();

    public Type GetEntityType();
    public List<Card> GetCards();

    public void InitAttack(IEnumerable<Card> options);
    public (CustomYieldInstruction, Func<Card>) ChooseAttack(IEnumerable<Card> options);

    public void InitTarget(IEnumerable<IBattleEntity> options);
    public (CustomYieldInstruction, Func<IBattleEntity>) ChooseTarget(IEnumerable<IBattleEntity> options);

    public AttackInfo Attack(Card attack, IBattleEntity target);
    public void ReceiveAttack(AttackInfo results);

    public ref Stats GetStats();

    public void Die();
}
