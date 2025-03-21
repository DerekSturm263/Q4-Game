using System;
using System.Collections.Generic;
using UnityEngine;

namespace Types.Miscellaneous
{
    [Serializable]
    public struct SaveData
    {
        [SerializeField] private List<Card> _cards;
        public readonly List<Card> Cards => _cards;

        [SerializeField] private List<Mask> _masks;
        public readonly List<Mask> Masks => _masks;
    }
}
