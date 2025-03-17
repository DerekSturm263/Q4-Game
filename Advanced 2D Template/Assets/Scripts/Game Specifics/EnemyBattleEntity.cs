using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBattleEntity : BattleEntity
{
    public override IEnumerator ChooseAttack(IEnumerable<Card> options)
    {
        return options.ElementAt(Random.Range(0, options.Count())) as IEnumerator;
    }

    public override IEnumerator ChooseTarget(IEnumerable<IBattleEntity> options)
    {
        return options.ElementAt(Random.Range(0, options.Count())) as IEnumerator;
    }
}
