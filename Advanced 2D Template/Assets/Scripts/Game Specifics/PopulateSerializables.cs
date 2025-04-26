using Helpers;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Types.Wrappers;

public abstract class PopulateSerializables<T> : MonoBehaviours.UI.Populate<Serializable<T>> where T : ISerializable
{
    protected override string Description(Serializable<T> item) => item.Description;

    protected override Dictionary<string, Predicate<Serializable<T>>> GetAllFilterModes()
    {
        return new()
        {
            ["All"] = (value) => true
        };
    }

    protected override Dictionary<string, Func<Serializable<T>, (string, object)>> GetAllGroupModes()
    {
        return new()
        {
            ["All"] = (value) => ("All", 0)
        };
    }

    protected override Dictionary<string, Comparison<Serializable<T>>> GetAllSortModes()
    {
        return new()
        {
            ["Last Edited"] = (lhs, rhs) => lhs.LastEditedDate.CompareTo(rhs.LastEditedDate)
        };
    }

    protected override bool IsEquipped(Serializable<T> item) => false;

    protected override bool IsNone(Serializable<T> item) => false;

    protected override bool IsRandom(Serializable<T> item) => false;

    protected override IEnumerable<Serializable<T>> LoadAll() => SerializationHelper.LoadAllFromDirectory<Serializable<T>>(default(T).GetFilePath());

    protected override Serializable<T> Random(IEnumerable<Serializable<T>> items)
    {
        return items.ElementAt(UnityEngine.Random.Range(0, items.Count()));
    }

    protected override void ReassignRandom(ref Serializable<T> random, IEnumerable<Serializable<T>> items)
    {
        random = Random(items);
    }
}
