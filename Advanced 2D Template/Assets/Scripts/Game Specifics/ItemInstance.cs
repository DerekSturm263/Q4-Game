using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    [SerializeField] private Item _item;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        InventoryController.Instance.AddItem(_item);
        Destroy(gameObject);
    }

    public static ItemInstance SpawnFromItem(ItemInstance prefab, Item item, Vector3 position, Quaternion rotation)
    {
        var instance = Instantiate(prefab);

        instance._item = item;
        instance.GetComponent<SpriteRenderer>().sprite = item.Texture;

        return instance;
    }
}
