using System;
using System.Collections.Generic;
using UnityEngine.Events;

public struct AttackInfo
{
    public enum HealthChangeResult
    {
        None,
        Dead
    }

    public IBattleEntity attacker;
    public IBattleEntity defender;

    public Card card;

    public HealthChangeResult result;
}
