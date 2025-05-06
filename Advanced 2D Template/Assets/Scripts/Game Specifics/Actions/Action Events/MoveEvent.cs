using System.Collections;
using UnityEngine;

public class MoveEvent : ActionEvent
{
    [SerializeField] private float _length;
    [SerializeField] private AnimationCurve _xCurve;
    [SerializeField] private AnimationCurve _yCurve;
    [SerializeField] private float _distance;

    public override IEnumerator Event(BattleController ctx)
    {
        Vector3 originalPos = ctx.Current.transform.position;
        Vector3 position = ctx.Current.Target.transform.position - (ctx.Current.Target.transform.position - originalPos).normalized * _distance;

        ctx.Current.Animator.SetFloat("Direction", Mathf.Sign(position.x - originalPos.x));

        for (float t = 0; t < _length; t += Time.deltaTime)
        {
            Vector3 newPos = new
            (
                Mathf.Lerp(originalPos.x, position.x, _xCurve.Evaluate(t * (1 / _length))),
                Mathf.Lerp(originalPos.y, position.y, _yCurve.Evaluate(t * (1 / _length))),
                Mathf.Lerp(originalPos.z, position.z, t * (1 / _length))
            );
            ctx.Current.transform.position = newPos;

            yield return new WaitForEndOfFrame();
        }

        ctx.Current.Animator.SetFloat("Direction", 0);
    }
}
