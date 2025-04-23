using System;
using System.Collections.Generic;
using UnityEngine;

namespace Types.Miscellaneous
{
    [Serializable]
    public struct SaveData
    {
        [Flags]
        public enum Abilities
        {
            Swim = 1 << 0,
            Break = 1 << 1,
            Bubble = 1 << 2,
            Grow = 1 << 3,
            Jump = 1 << 4
        }

        [SerializeField] private List<Item> _items;
        public readonly List<Item> Items => _items;

        [SerializeField] private Mask _mask;
        public readonly Mask Mask => _mask;
        public void SetMask(Mask mask) => _mask = mask;

        [SerializeField] private float _currentHealth;
        public readonly float CurrentHealth => _currentHealth;
        public void SetCurrentHealth(float currentHealth) => _currentHealth = currentHealth;

        [SerializeField] private float _maxHealth;
        public readonly float MaxHealth => _maxHealth;
        public void SetMaxHealth(float maxHealth) => _maxHealth = maxHealth;

        [SerializeField] private float _attack;
        public readonly float Attack => _attack;
        public void SetAttack(float attack) => _attack = attack;
        
        [SerializeField] private float _defense;
        public readonly float Defense => _defense;
        public void SetDefense(float defense) => _defense = defense;

        [SerializeField] private List<string> _storyData;
        public readonly List<string> StoryData => _storyData;

        [SerializeField] private Abilities _playerAbilities;
        public readonly Abilities PlayerAbilities => _playerAbilities;

        [SerializeField] private Vector2 _position;
        public readonly Vector2 Position => _position;
        public void SetPosition(Vector2 position) => _position = position;

        [SerializeField] private Types.Collections.Dictionary<Quest, bool> _questsCompleted;
        public readonly Types.Collections.Dictionary<Quest, bool> QuestsCompleted => _questsCompleted;
    }
}
