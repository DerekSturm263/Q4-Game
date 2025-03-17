using UnityEngine;

[CreateAssetMenu(fileName = "New Entity Stats", menuName = "Game/Entity Stats")]
public class EntityStats : ScriptableObject
{
    [TextArea][SerializeField] private string _description;
    public string Description => _description;

    [SerializeField] private Stats _stats;
    public Stats Stats => _stats;
}
