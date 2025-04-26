using SingletonBehaviours;
using System;
using System.Collections.Generic;

public class PopulateQuests : PopulateAssets<Quest>
{
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
}
