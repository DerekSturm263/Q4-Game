using Types.Miscellaneous;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Mask", menuName = "Game/Mask")]
public class Mask : Asset
{
    [SerializeField] private UnityEvent<PlayerMovement> _inputAction;
    public void InvokeAction(PlayerMovement action) => _inputAction?.Invoke(action);

    [SerializeField] private AnyGroup _data;

    public void SpitWater(PlayerMovement player)
    {
        if (_data.TryGet("Prefab", out GameObject prefab) && _data.TryGet("Speed", out float speed) && _data.TryGet("Offset", out float offset))
        {
            GameObject bubble = Instantiate(prefab, player.transform.position + player.InteractOffset * offset, Quaternion.identity);
            bubble.GetComponent<Rigidbody2D>().linearVelocity = player.LookDirection * speed;
        }
    }
}
