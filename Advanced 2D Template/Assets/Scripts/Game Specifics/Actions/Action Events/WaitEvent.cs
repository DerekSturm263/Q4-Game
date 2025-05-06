using System.Collections;
using UnityEngine;

public class WaitEvent : ActionEvent
{
    [SerializeField] private float _time;

    public override IEnumerator Event(BattleController ctx)
    {
        yield return new WaitForSeconds(_time);
    }
}
