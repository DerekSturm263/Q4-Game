using SingletonBehaviours;
using UnityEngine;

public class InventoryController : Types.SingletonBehaviour<InventoryController>
{
    public void AddItem(Asset item)
    {
        SaveDataController.Instance.CurrentData.Items.Add(item);
        Debug.Log($"{item.name} acquired");
    }

    public void RemoveItem(Asset item)
    {
        SaveDataController.Instance.CurrentData.Items.Remove(item);
        Debug.Log($"{item.name} lost");
    }
}
