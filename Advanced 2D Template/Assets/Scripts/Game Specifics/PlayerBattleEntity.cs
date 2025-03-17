using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleEntity : BattleEntity
{
    private Card _currentCard;
    private IBattleEntity _battleEntity;

    public override IEnumerator ChooseAttack(IEnumerable<Card> options)
    {
        yield return new WaitUntil(() => _currentCard != null);
        yield return _currentCard.Invoke();

        _currentCard = null;
    }

    public override IEnumerator ChooseTarget(IEnumerable<IBattleEntity> options)
    {
        yield return new WaitUntil(() => _battleEntity != null);
        yield return _battleEntity;

        _battleEntity = null;
    }
}
