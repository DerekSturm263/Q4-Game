using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattlePlayer : BattleEntity, ISubmitHandler
{
    private static Card _currentCard;
    private static IBattleEntity _currentTarget;

    public override void InitAttack(IEnumerable<Card> options)
    {
        _currentCard = null;
    }

    public override (CustomYieldInstruction, Func<Card>) ChooseAttack(IEnumerable<Card> options)
    {
        return (new WaitUntil(() => _currentCard != null), () => _currentCard);
    }

    public override void InitTarget(IEnumerable<IBattleEntity> options)
    {
        _currentTarget = null;
        EventSystem.current.SetSelectedGameObject(BattleController.Instance.EnemySpots[0].gameObject);
    }

    public override (CustomYieldInstruction, Func<IBattleEntity>) ChooseTarget(IEnumerable<IBattleEntity> options)
    {
        return (new WaitUntil(() => _currentTarget != null), () => _currentTarget);
    }

    public static void SelectCard(Card card)
    {
        _currentCard = card;
    }

    public static void SelectTarget(IBattleEntity target)
    {
        _currentTarget = target;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        SelectTarget(this);
    }
}
