using System.Collections.Generic;
using Types.Miscellaneous;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Battle Settings", menuName = "Game/Battle Settings")]
public class BattleSettings : ScriptableObject
{
    [SerializeField] private List<Tuple<EntityStats, Range<int>>> _stats;
    public List<Tuple<EntityStats, Range<int>>> Stats => _stats;

    [SerializeField] private BattleEnvironment _environment;
    public BattleEnvironment Environment => _environment;

    [SerializeField] private List<Tuple<Asset, Range<int>>> _rewards;
    public List<Tuple<Asset, Range<int>>> Rewards => _rewards;

    [SerializeField] private UnityEvent _onComplete;
    public void InvokeOnComplete() => _onComplete.Invoke();

    public static BattleSettings CreateTest()
    {
        var settings = CreateInstance<BattleSettings>();

        settings._stats = new()
        {
            new(EntityStats.CreateTest(new("Test", null, 20, 5, 3, new() {  })), new(1, 2))
        };

        settings._environment = CreateInstance<BattleEnvironment>();

        return settings;
    }
}
