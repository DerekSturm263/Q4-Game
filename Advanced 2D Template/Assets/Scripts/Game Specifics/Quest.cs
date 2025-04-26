using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Quest", menuName = "Game/Quest")]
public class Quest : Asset
{
    [SerializeField] private UnityEvent<PlayerMovement> _onComplete;
    public void Complete(PlayerMovement playerMovement) => _onComplete.Invoke(playerMovement);
}
