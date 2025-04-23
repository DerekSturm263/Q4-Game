using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Game/Inventory Item")]
public class InventoryItem : Item
{
    [SerializeField] private UnityEvent<PlayerMovement> _action;
    public void InvokeAction(PlayerMovement action) => _action?.Invoke(action);
}
