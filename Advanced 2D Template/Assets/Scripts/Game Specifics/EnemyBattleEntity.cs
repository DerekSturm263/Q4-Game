using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBattleEntity : BattleEntity
{
    public override void InitAttack(IEnumerable<Card> options)
    {

    }

    public override (CustomYieldInstruction, Func<Card>) ChooseAttack(IEnumerable<Card> options)
    {
        return (new WaitUntil(() => true), () => options.ElementAt(UnityEngine.Random.Range(0, options.Count())));
    }

    public override void InitTarget(IEnumerable<IBattleEntity> options)
    {

    }

    public override (CustomYieldInstruction, Func<IBattleEntity>) ChooseTarget(IEnumerable<IBattleEntity> options)
    {
        return (new WaitUntil(() => true), () => options.ElementAt(UnityEngine.Random.Range(0, options.Count())));
    }
}
