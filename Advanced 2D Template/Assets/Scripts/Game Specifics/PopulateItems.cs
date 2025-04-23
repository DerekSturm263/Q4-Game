using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulateItems : MonoBehaviours.UI.Populate<Item>
{
    protected override string Description(Item item) => item.Description;

    protected override Dictionary<string, Predicate<Item>> GetAllFilterModes()
    {
        return new()
        {
            ["All"] = (value) => true
        };
    }

    protected override Dictionary<string, Func<Item, (string, object)>> GetAllGroupModes()
    {
        return new()
        {
            ["All"] = (value) => ("All", 0)
        };
    }

    protected override Dictionary<string, Comparison<Item>> GetAllSortModes()
    {
        return new()
        {
            ["Name"] = (lhs, rhs) => lhs.name.CompareTo(rhs.name)
        };
    }

    protected override Sprite Icon(Item item) => item.Icon;

    protected override bool IsEquipped(Item item) => false;

    protected override bool IsNone(Item item) => false;

    protected override bool IsRandom(Item item) => false;

    protected override IEnumerable<Item> LoadAll() => SingletonBehaviours.SaveDataController.Instance.CurrentData.Items;

    protected override string Name(Item item) => item.name;

    protected override Item Random(IEnumerable<Item> items)
    {
        return items.ElementAt(UnityEngine.Random.Range(0, items.Count()));
    }

    protected override void ReassignRandom(ref Item random, IEnumerable<Item> items)
    {
        random = Random(items);
    }
}
