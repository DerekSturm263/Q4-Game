using Types.Miscellaneous;
using UnityEngine;

[CreateAssetMenu(fileName = "New Corridor Settings", menuName = "Game/Dungeon/Corridor Settings")]
public class CorrdiorSettings : ScriptableObject
{
    public enum CorridorType
    {
        Staircase,
        StraightLine,
        Curvy
    }

    [SerializeField] private Range<int> _thickness;
    [SerializeField] private CorridorType _type;
}
