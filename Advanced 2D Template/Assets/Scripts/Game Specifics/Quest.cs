using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Quest", menuName = "Game/Quest")]
public class Quest : ScriptableObject
{
    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;

    [TextArea][SerializeField] private string _description;
    public string Description => _description;

    [SerializeField] private UnityEvent<PlayerMovement> _onComplete;
    public void Complete(PlayerMovement playerMovement) => _onComplete.Invoke(playerMovement);
}
