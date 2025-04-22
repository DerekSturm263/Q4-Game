using System;
using System.Collections.Generic;
using UnityEngine;

public class PopulateItems : MonoBehaviours.UI.Populate<Item>
{
    protected override string Description(Item item) => item.Description;

    protected override Dictionary<string, Predicate<Item>> GetAllFilterModes()
    {
        throw new NotImplementedException();
    }

    protected override Dictionary<string, Func<Item, (string, object)>> GetAllGroupModes()
    {
        throw new NotImplementedException();
    }

    protected override Dictionary<string, Comparison<Item>> GetAllSortModes()
    {
        throw new NotImplementedException();
    }

    protected override Sprite Icon(Item item)
    {
        throw new NotImplementedException();
    }

    protected override bool IsEquipped(Item item)
    {
        throw new NotImplementedException();
    }

    protected override bool IsNone(Item item)
    {
        throw new NotImplementedException();
    }

    protected override bool IsRandom(Item item)
    {
        throw new NotImplementedException();
    }

    protected override IEnumerable<Item> LoadAll()
    {
        throw new NotImplementedException();
    }

    protected override string Name(Item item)
    {
        throw new NotImplementedException();
    }

    protected override Item Random(IEnumerable<Item> items)
    {
        throw new NotImplementedException();
    }

    protected override void ReassignRandom(ref Item random, IEnumerable<Item> items)
    {
        throw new NotImplementedException();
    }
}
