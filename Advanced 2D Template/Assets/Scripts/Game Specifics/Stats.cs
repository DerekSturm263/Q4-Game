using System;
using UnityEngine;

[Serializable]
public struct Stats
{
    [SerializeField] private string _name;
    public readonly string Name => _name;

    [SerializeField] private float _currentHealth;
    public readonly float CurrentHealth => _currentHealth;
    public readonly bool IsAlive => _currentHealth > 0;

    [SerializeField] private float _maxHealth;
    public readonly float MaxHealth => _maxHealth;

    [SerializeField] private float _attack;
    public readonly float Attack => _attack;

    [SerializeField] private float _defense;
    public readonly float Defense => _defense;

    public Stats(string name, float health, float attack, float defense)
    {
        _name = name;
        _currentHealth = health;
        _maxHealth = health;
        _attack = attack;
        _defense = defense;
    }

    public ActionInfo.HealthChangeResult ModifyHealth(float delta)
    {
        _currentHealth += delta;

        if (_currentHealth > _maxHealth)
            _currentHealth = _maxHealth;

        return _currentHealth <= 0 ? ActionInfo.HealthChangeResult.Dead : ActionInfo.HealthChangeResult.None;
    }
}
