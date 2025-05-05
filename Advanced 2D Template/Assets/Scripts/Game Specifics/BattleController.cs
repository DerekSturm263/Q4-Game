using SingletonBehaviours;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Types.Scene;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class BattleController : Types.SingletonBehaviour<BattleController>
{
    [SerializeField] private BattlePlayer _playerTemplate;
    [SerializeField] private BattleEnemy _enemyTemplate;

    [SerializeField] private List<Transform> _playerSpots;
    public List<Transform> PlayerSpots => _playerSpots;

    [SerializeField] private List<Transform> _enemySpots;
    public List<Transform> EnemySpots => _playerSpots;

    [SerializeField] private Light2D _spotlight;
    [SerializeField] private Vector3 _lightOffset;

    private readonly List<BattleEntity> _entities = new();

    private BattleEntity _current;
    public BattleEntity Current => _current;

    [SerializeField] private SceneLoadSettings _backToOverworld;
    public SceneLoadSettings BackToOverworld => _backToOverworld;

    public IEnumerable<IBattleEntity> GetFromType(IBattleEntity.Type type) => _entities.Where(item => item.GetEntityType() == type);
    public IEnumerable<IBattleEntity> GetFromTypeAlive(IBattleEntity.Type type) => _entities.Where(item => item.GetEntityType() == type && item.GetStats().IsAlive);
    private int GetNumberAlive(IBattleEntity.Type type) => GetFromType(type).Count(item => item.GetStats().IsAlive);
    private bool GetIsTeamAlive(IBattleEntity.Type type) => GetNumberAlive(type) > 0;

    private void Start()
    {
        BattleSettings settings = SceneController.Instance.GetSceneParameter<ScriptableObject>("Value") as BattleSettings;

        if (!settings)
            settings = BattleSettings.CreateTest();

        List<EntityStats> enemies = new();

        foreach (var enemy in settings.Stats)
        {
            int count = Random.Range(enemy.Item2.Min, enemy.Item2.Max + 1);

            for (int i = 0; i < count; ++i)
            {
                enemies.Add(enemy.Item1);
            }
        }

        BattleSetup setup = new
        (
            SaveDataController.Instance.CurrentData.Stats,
            enemies,
            settings.Environment
        );

        StartBattle(setup);
    }

    private void Update()
    {
        _spotlight.gameObject.SetActive(_current is BattlePlayer);

        if (_current is BattlePlayer)
        {
            if (_current.CurrentAction is null)
                _spotlight.transform.position = _current.transform.position + _lightOffset;
            else
                _spotlight.transform.position = EventSystem.current.currentSelectedGameObject.transform.position + _lightOffset;
        }
    }

    public void StartBattle(BattleSetupAsset setup) => StartBattle(setup.Value);
    public void StartBattle(BattleSetup setup) => StartCoroutine(Battle(setup));

    IEnumerator Battle(BattleSetup setup)
    {
        PopulateEntities(setup);

        while (GetIsTeamAlive(IBattleEntity.Type.Human) && GetIsTeamAlive(IBattleEntity.Type.AI))
        {
            foreach (var entity in _entities)
            {
                if (!entity.GetStats().IsAlive)
                    continue;

                _current = entity;
                yield return entity.DoTurn(this);
            }
        }

        SceneController.Instance.Load(_backToOverworld);
    }

    public void PopulateEntities(BattleSetup setup)
    {
        _entities.Clear();

        int playerIndex = 0;
        _entities.AddRange(setup.Players.Select(item =>
        {
            var player = Instantiate(_playerTemplate, _playerSpots[playerIndex++]);
            player.SetStats(item);

            return player;
        }));

        foreach (var spot in _playerSpots.Where(item => item.childCount == 0))
            spot.gameObject.SetActive(false);

        int enemyIndex = 0;
        _entities.AddRange(setup.Enemies.Select(item =>
        {
            var enemy = Instantiate(_enemyTemplate, _enemySpots[enemyIndex++]);
            enemy.SetStats(item);

            enemy.name = $"{item.Name} {(char)('A' + enemyIndex - 1)}";

            return enemy;
        }));

        foreach (var spot in _enemySpots.Where(item => item.childCount == 0))
            spot.gameObject.SetActive(false);
    }
}
