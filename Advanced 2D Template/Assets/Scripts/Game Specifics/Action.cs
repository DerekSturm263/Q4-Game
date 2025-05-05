using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "Game/Action")]
public class Action : ScriptableObject
{
    public enum ActionType
    {
        Swipe
    }

    [SerializeField] private ActionType _type;
    public ActionType Type => _type;

    [SerializeField] private float _damage;
    [SerializeField] private float _attackDelay;

    public Func<BattleController, IEnumerator> GetAction()
    {
        return _type switch
        {
            ActionType.Swipe => SwipeEnumerator,
            _ => default
        };
    }

    private IEnumerator SwipeEnumerator(BattleController ctx)
    {
        Vector3 originalPos = ctx.Current.transform.position;
        Vector3 offset = (ctx.Current.Target.transform.position - originalPos).normalized * 1.5f;
        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            ctx.Current.transform.position = Vector3.Lerp(originalPos, ctx.Current.Target.transform.position - offset, t * 2);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(_attackDelay);

        ctx.Current.Animator.SetTrigger("Swipe");

        yield return new WaitForSeconds(0.25f);

        ctx.Current.Target.TakeDamage(_damage);

        yield return new WaitForSeconds(0.5f);

        Vector3 originalPos3 = ctx.Current.transform.position;
        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            ctx.Current.transform.position = Vector3.Lerp(originalPos3, originalPos, t * 2);
            yield return new WaitForEndOfFrame();
        }
    }
}
