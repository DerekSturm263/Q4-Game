using System.Collections;
using UnityEngine;

public class DamageEvent : ActionEvent
{
    [SerializeField] private string _attackName;
    [SerializeField] private float _delay;
    [SerializeField] private float _damage;
    
    public override IEnumerator Event(BattleController ctx)
    {
        ctx.Current.Animator.SetTrigger(_attackName);

        yield return new WaitForSeconds(_delay);

        ctx.Current.Target.TakeDamage(_damage);
    }
}
