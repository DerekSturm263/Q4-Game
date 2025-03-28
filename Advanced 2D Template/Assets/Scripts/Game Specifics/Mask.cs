using Types.Collections;
using Types.Miscellaneous;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Mask", menuName = "Game/Mask")]
public class Mask : Item
{
    [SerializeField] private InputAction _input;
    public InputAction Input => _input;

    [SerializeField] private UnityEvent<PlayerMovement> _inputAction;
    public void InvokeAction(PlayerMovement action) => _inputAction?.Invoke(action);

    //[SerializeField] private Dictionary<string, Any> _data;

    public Any test1;
    public Any test2;
    public Any test3;

    public Dictionary<string, GameObject> test4;

    public void SpitWater(PlayerMovement player)
    {
        /*if (_data.TryGetValue("Prefab", out Any prefab) && _data.TryGetValue("Speed", out Any speed))
        {
            GameObject bubble = Instantiate(prefab.Get<GameObject>(), player.transform.position + player.InteractOffset, Quaternion.identity);
            bubble.GetComponent<Rigidbody2D>().linearVelocity = player.LookDirection * speed.Get<float>();
        }*/
    }
}
