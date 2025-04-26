using System.Collections.Generic;

public class PopulateItems : PopulateAssets<Asset>
{
    protected override IEnumerable<Asset> LoadAll() => SingletonBehaviours.SaveDataController.Instance.CurrentData.Items;
}
