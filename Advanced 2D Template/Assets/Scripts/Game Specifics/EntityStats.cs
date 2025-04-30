using UnityEngine;

[CreateAssetMenu(fileName = "New Entity Stats", menuName = "Game/Entity Stats")]
public class EntityStats : Asset
{
    [SerializeField] private Stats _stats;
    public Stats Stats => _stats;

    public static EntityStats CreateTest(Stats stats)
    {
        var entity = CreateInstance<EntityStats>();
        entity._stats = stats;

        return entity;
    }
}
