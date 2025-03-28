using System;
using System.Collections.Generic;
using UnityEngine;

namespace Types.Miscellaneous
{
    [Serializable]
    public struct SaveData
    {
        [SerializeField] private List<Item> _items;
        public readonly List<Item> Items => _items;

        [SerializeField] private float _currentHealth;
        public readonly float CurrentHealth => _currentHealth;

        [SerializeField] private float _maxHealth;
        public readonly float MaxHealth => _maxHealth;

        [SerializeField] private float _attack;
        public readonly float Attack => _attack;
        
        [SerializeField] private float _defense;
        public readonly float Defense => _defense;
    }
}
