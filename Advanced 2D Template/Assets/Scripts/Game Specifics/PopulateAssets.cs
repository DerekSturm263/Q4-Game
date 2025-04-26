using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PopulateAssets<T> : MonoBehaviours.UI.Populate<T> where T : Asset
{
    [SerializeField] private List<T> _assets;

    protected override string Description(T item) => item.Description;

    protected override Dictionary<string, Predicate<T>> GetAllFilterModes()
    {
        return new()
        {
            ["All"] = (value) => true
        };
    }

    protected override Dictionary<string, Func<T, (string, object)>> GetAllGroupModes()
    {
        return new()
        {
            ["All"] = (value) => ("All", 0)
        };
    }

    protected override Dictionary<string, Comparison<T>> GetAllSortModes()
    {
        return new()
        {
            ["Name"] = (lhs, rhs) => lhs.name.CompareTo(rhs.name)
        };
    }

    protected override Sprite Icon(T item) => item.Icon;

    protected override bool IsEquipped(T item) => false;

    protected override bool IsNone(T item) => false;

    protected override bool IsRandom(T item) => false;

    protected override IEnumerable<T> LoadAll() => _assets;

    protected override string Name(T item) => item.name;

    protected override T Random(IEnumerable<T> items)
    {
        return items.ElementAt(UnityEngine.Random.Range(0, items.Count()));
    }

    protected override void ReassignRandom(ref T random, IEnumerable<T> items)
    {
        random = Random(items);
    }
}
