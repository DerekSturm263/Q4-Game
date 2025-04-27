using System.Collections.Generic;
using Types.Miscellaneous;
using UnityEngine;

[CreateAssetMenu(fileName = "New Battle Settings", menuName = "Game/Battle Settings")]
public class BattleSettings : ScriptableObject
{
    [SerializeField] private List<Tuple<EntityStats, Range<int>>> _stats;
    public List<Tuple<EntityStats, Range<int>>> Stats => _stats;

    [SerializeField] private BattleEnvironment _environment;
    public BattleEnvironment Environment => _environment;
}
