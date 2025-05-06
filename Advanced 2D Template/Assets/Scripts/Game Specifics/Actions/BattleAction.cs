using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "Battle Action", menuName = "Game/Battle Action")]
public class BattleAction : ScriptableObject
{
    [SerializeField] private List<ActionEvent> _events;
    public List<ActionEvent> Events => _events;

    public IEnumerator DoAction(BattleController ctx)
    {
        if (ctx.Current is BattlePlayer)
        {
            EventSystem.current.SetSelectedGameObject((ctx.GetFromTypeAlive(IBattleEntity.Type.AI).ElementAt(0) as BattleEntity).gameObject);
            yield return new WaitUntil(() => ctx.Current.Target);
        }
        else
        {
            var players = ctx.PlayerSpots[0].parent.GetComponentsInChildren<BattlePlayer>(false);
            ctx.Current.SetTarget(players.ElementAt(Random.Range(0, players.Length)));
        }

        Vector3 position = ctx.Current.transform.position;

        foreach (var action in _events)
        {
            yield return action.Event(ctx);
        }

        if (ctx.Current.transform.position != position)
            yield return BackToStart(ctx, position);
    }

    private IEnumerator BackToStart(BattleController ctx, Vector3 position)
    {
        Vector3 originalPos = ctx.Current.transform.position;

        ctx.Current.Animator.SetFloat("Direction", Mathf.Sign(position.x - originalPos.x));

        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            ctx.Current.transform.position = Vector3.Lerp(originalPos, position, t * (1 / 0.5f));

            yield return new WaitForEndOfFrame();
        }

        ctx.Current.Animator.SetFloat("Direction", 0);
    }
}
