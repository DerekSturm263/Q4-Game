using UnityEngine;
using UnityEngine.EventSystems;

public class BattleEnemy : BattleEntity
{
    public override void InitAction(BattleController ctx)
    {

    }

    public override (CustomYieldInstruction, ActionInfo) ChooseAction(BattleController ctx)
    {
        return (new WaitUntil(() => true), default);
    }

    public override void OnSubmit(BaseEventData eventData)
    {

    }
}
