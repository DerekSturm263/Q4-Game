using System.Collections;
using UnityEngine;

public class ToggleDamageEvent : ActionEvent
{
    [SerializeField] private string _attackName;
    [SerializeField] private float _damage;
    [SerializeField] private bool _state;

    public override IEnumerator Event(BattleController ctx)
    {
        ctx.Current.Animator.SetBool(_attackName, _state);

        yield return null;
    }
}
