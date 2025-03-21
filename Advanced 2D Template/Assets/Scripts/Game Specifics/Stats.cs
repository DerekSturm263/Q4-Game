using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct Stats
{
    [SerializeField] private string _name;
    public readonly string Name => _name;

    [SerializeField] private float _health;
    public readonly float Health => _health;
    public readonly bool IsAlive => _health > 0;

    public AttackInfo.HealthChangeResult ModifyHealth(float delta)
    {
        _health += delta;

        return _health <= 0 ? AttackInfo.HealthChangeResult.Dead : AttackInfo.HealthChangeResult.None;
    }

    [SerializeField] private float _attack;
    public readonly float Attack => _attack;

    [SerializeField] private float _defense;
    public readonly float Defense => _defense;

    [SerializeField] private List<Card> _cards;
    public readonly List<Card> Cards => _cards;
}
