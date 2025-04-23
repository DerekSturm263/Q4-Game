using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    [SerializeField] private Item _item;
    [SerializeField] private ParticleSystem _collectEffect;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (InventoryController.Instance)
            InventoryController.Instance.AddItem(_item);
    
        Instantiate(_collectEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public static ItemInstance SpawnFromItem(ItemInstance prefab, Item item, Vector3 position, Quaternion rotation)
    {
        var instance = Instantiate(prefab);

        instance._item = item;
        instance.GetComponent<SpriteRenderer>().sprite = item.Icon;

        return instance;
    }
}
