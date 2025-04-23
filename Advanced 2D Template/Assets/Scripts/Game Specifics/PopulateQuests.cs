using SingletonBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulateQuests : MonoBehaviours.UI.Populate<Quest>
{
    [SerializeField] private List<Quest> _quests;

    protected override string Description(Quest item) => item.Description;

    protected override Dictionary<string, Predicate<Quest>> GetAllFilterModes()
    {
        return new() {
            ["All"] = (value) => true,
            ["Complete"] = (value) => SaveDataController.Instance.CurrentData.QuestsCompleted[value],
            ["Incomplete"] = (value) => !SaveDataController.Instance.CurrentData.QuestsCompleted[value]
        };
    }

    protected override Dictionary<string, Func<Quest, (string, object)>> GetAllGroupModes()
    {
        return new()
        {
            ["All"] = (value) => ("All", 0)
        };
    }

    protected override Dictionary<string, Comparison<Quest>> GetAllSortModes()
    {
        return new()
        {
            ["Name"] = (lhs, rhs) => lhs.name.CompareTo(rhs.name)
        };
    }

    protected override Sprite Icon(Quest item) => item.Icon;

    protected override bool IsEquipped(Quest item) => false;

    protected override bool IsNone(Quest item) => false;

    protected override bool IsRandom(Quest item) => false;

    protected override IEnumerable<Quest> LoadAll() => _quests;

    protected override string Name(Quest item) => item.name;

    protected override Quest Random(IEnumerable<Quest> items)
    {
        return items.ElementAt(UnityEngine.Random.Range(0, items.Count()));
    }

    protected override void ReassignRandom(ref Quest random, IEnumerable<Quest> items)
    {
        random = Random(items);
    }
}
