using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public struct BattleSetup
{
    [SerializeField] private List<Stats> _players;
    public readonly IEnumerable<Stats> Players => _players;

    [SerializeField] private List<Stats> _enemies;
    public readonly IEnumerable<Stats> Enemies => _enemies;

    public readonly IEnumerable<Stats> Entities => Players.Concat(Enemies);

    [SerializeField] private BattleEnvironment _environment;
    public readonly BattleEnvironment Environment => _environment;

    public BattleSetup(List<Stats> players, List<Stats> enemies, BattleEnvironment environment)
    {
        _players = players;
        _enemies = enemies;
        _environment = environment;
    }

    public BattleSetup(List<EntityStats> players, List<EntityStats> enemies, BattleEnvironment environment)
    {
        _players = players.Select(item => item.Stats).ToList();
        _enemies = enemies.Select(item => item.Stats).ToList();
        _environment = environment;
    }

    public BattleSetup(List<Stats> players, List<EntityStats> enemies, BattleEnvironment environment)
    {
        _players = players;
        _enemies = enemies.Select(item => item.Stats).ToList();
        _environment = environment;
    }

    public BattleSetup(List<EntityStats> players, List<Stats> enemies, BattleEnvironment environment)
    {
        _players = players.Select(item => item.Stats).ToList();
        _enemies = enemies;
        _environment = environment;
    }
}
