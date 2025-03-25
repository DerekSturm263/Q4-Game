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
    }
}
