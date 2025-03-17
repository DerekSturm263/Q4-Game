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
}
