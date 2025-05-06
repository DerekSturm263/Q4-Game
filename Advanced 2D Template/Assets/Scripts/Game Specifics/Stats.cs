using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Stats
{
    [SerializeField] private string _name;
    public readonly string Name => _name;

    [SerializeField] private RuntimeAnimatorController _animatorController;
    public readonly RuntimeAnimatorController AnimatorController => _animatorController;

    [SerializeField] private float _currentHealth;
    public readonly float CurrentHealth => _currentHealth;
    public readonly bool IsAlive => _currentHealth > 0;

    [SerializeField] private float _maxHealth;
    public readonly float MaxHealth => _maxHealth;

    [SerializeField] private float _attack;
    public readonly float Attack => _attack;

    [SerializeField] private float _defense;
    public readonly float Defense => _defense;

    [SerializeField] private List<BattleAction> _actions;
    public readonly List<BattleAction> Actions => _actions;

    public Stats(string name, RuntimeAnimatorController animatorController, float health, float attack, float defense, List<BattleAction> actions)
    {
        _name = name;
        _animatorController = animatorController;
        _currentHealth = health;
        _maxHealth = health;
        _attack = attack;
        _defense = defense;
        _actions = actions;
    }

    public ActionInfo.HealthChangeResult ModifyHealth(float amount)
    {
        _currentHealth += amount;

        if (_currentHealth > _maxHealth)
            _currentHealth = _maxHealth;

        return _currentHealth <= 0 ? ActionInfo.HealthChangeResult.Dead : ActionInfo.HealthChangeResult.None;
    }
}
