using System.Collections;
using System.Linq;
using UnityEngine;

public class ChooseTargetEvent : ActionEvent
{
    public override IEnumerator Event(BattleController ctx)
    {
        var players = ctx.PlayerSpots[0].parent.GetComponentsInChildren<BattlePlayer>(false);
        ctx.Current.SetTarget(players.ElementAt(Random.Range(0, players.Length)));

        yield return null;
    }
}
