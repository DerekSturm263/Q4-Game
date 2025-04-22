using System;
using System.Collections.Generic;
using UnityEngine;

public class PopulateQuests : MonoBehaviours.UI.Populate<Quest>
{
    protected override string Description(Quest item) => item.Description;

    protected override Dictionary<string, Predicate<Quest>> GetAllFilterModes()
    {
        throw new NotImplementedException();
    }

    protected override Dictionary<string, Func<Quest, (string, object)>> GetAllGroupModes()
    {
        throw new NotImplementedException();
    }

    protected override Dictionary<string, Comparison<Quest>> GetAllSortModes()
    {
        throw new NotImplementedException();
    }

    protected override Sprite Icon(Quest item)
    {
        throw new NotImplementedException();
    }

    protected override bool IsEquipped(Quest item)
    {
        throw new NotImplementedException();
    }

    protected override bool IsNone(Quest item)
    {
        throw new NotImplementedException();
    }

    protected override bool IsRandom(Quest item)
    {
        throw new NotImplementedException();
    }

    protected override IEnumerable<Quest> LoadAll()
    {
        throw new NotImplementedException();
    }

    protected override string Name(Quest item)
    {
        throw new NotImplementedException();
    }

    protected override Quest Random(IEnumerable<Quest> items)
    {
        throw new NotImplementedException();
    }

    protected override void ReassignRandom(ref Quest random, IEnumerable<Quest> items)
    {
        throw new NotImplementedException();
    }
}
