using System.Collections;
using UnityEngine;

public class BattleEnemy : BattleEntity
{
    public override IEnumerator DoTurn(BattleController ctx)
    {
        BattlePlayer.SetCanEvade(true);

        _currentAction = null;
        _target = null;

        _currentAction = _currentStats.Actions[Random.Range(0, _currentStats.Actions.Count)].DoAction;

        yield return _currentAction.Invoke(ctx);

        _currentAction = null;
        _target = null;

        BattlePlayer.SetCanEvade(false);
    }
}
