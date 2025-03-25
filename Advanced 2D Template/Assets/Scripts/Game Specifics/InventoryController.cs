using SingletonBehaviours;
using UnityEditor;
using UnityEngine;

public class InventoryController : Types.SingletonBehaviour<InventoryController>
{
    public void AddItem(Item item)
    {
        SaveDataController.Instance.CurrentData.Items.Add(item);
        Debug.Log($"{item.name} acquired");
    }

    public void RemoveItem(Item item)
    {
        SaveDataController.Instance.CurrentData.Items.Remove(item);
        Debug.Log($"{item.name} lost");
    }
}
