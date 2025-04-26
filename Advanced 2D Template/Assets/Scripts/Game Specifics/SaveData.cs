using Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Types.Miscellaneous
{
    [Serializable]
    public struct SaveData : ISerializable
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

        [SerializeField] private List<Stats> _stats;
        public readonly List<Stats> Stats => _stats;

        [SerializeField] private List<Asset> _items;
        public readonly List<Asset> Items => _items;

        [SerializeField] private Mask _mask;
        public readonly Mask Mask => _mask;
        public void SetMask(Mask mask) => _mask = mask;

        [SerializeField] private List<string> _storyData;
        public readonly List<string> StoryData => _storyData;

        [SerializeField] private Abilities _playerAbilities;
        public readonly Abilities PlayerAbilities => _playerAbilities;

        [SerializeField] private Vector2 _position;
        public readonly Vector2 Position => _position;
        public void SetPosition(Vector2 position) => _position = position;

        [SerializeField] private Collections.Dictionary<Quest, bool> _questsCompleted;
        public readonly Collections.Dictionary<Quest, bool> QuestsCompleted => _questsCompleted;

        [SerializeField] private float _time;
        public readonly float Time => _time;
        public void SetTime(float time) => _time = time;

        [SerializeField] private Collections.Dictionary<string, Tuple<int, int>> _interactStates;
        public readonly Collections.Dictionary<string, Tuple<int, int>> InteractStates => _interactStates;

        public readonly string GetFilePath() => $"{Application.persistentDataPath}/SaveData";
    }
}
