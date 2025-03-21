using SingletonBehaviours;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : Types.SingletonBehaviour<BattleController>
{
    [SerializeField] private PlayerBattleEntity _playerTemplate;
    [SerializeField] private EnemyBattleEntity _enemyTemplate;

    [SerializeField] private List<Transform> _playerSpots;
    public List<Transform> PlayerSpots => _playerSpots;

    [SerializeField] private List<Transform> _enemySpots;
    public List<Transform> EnemySpots => _playerSpots;

    [SerializeField] private Button _cardTemplate;
    [SerializeField] private Button _maskTemplate;

    [SerializeField] private LayoutGroup _cards;
    [SerializeField] private LayoutGroup _masks;

    private readonly List<IBattleEntity> _entities = new();

    private IEnumerable<IBattleEntity> GetFromType(IBattleEntity.Type type) => _entities.Where(item => item.GetEntityType() == type);
    private int GetNumberAlive(IBattleEntity.Type type) => GetFromType(type).Count();

    private void Start()
    {
        BattleSetup setup = new(new List<Stats>() { new() }, SingletonBehaviours.SceneController.Instance.GetSceneParameter<List<EntityStats>>("Stats"));

        PopulateCards();
        PopulateMasks();
        StartBattle(setup);
    }

    public void StartBattle(BattleSetupAsset setup) => StartBattle(setup.Value);
    public void StartBattle(BattleSetup setup) => StartCoroutine(Battle(setup));

    IEnumerator Battle(BattleSetup setup)
    {
        _entities.Clear();

        int playerIndex = 0;
        _entities.AddRange(setup.Players.Select(item =>
        {
            var player = Instantiate(_playerTemplate, _playerSpots[playerIndex++]);
            player.SetStats(item);

            return player;
        }));

        int enemyIndex = 0;
        _entities.AddRange(setup.Enemies.Select(item =>
        {
            var enemy = Instantiate(_enemyTemplate, _enemySpots[enemyIndex++]);
            enemy.SetStats(item);

            enemy.name = $"{ item.Name} {(char)('A' + enemyIndex - 1)}";

            return enemy;
        }));

        while (GetNumberAlive(IBattleEntity.Type.Human) > 0 || GetNumberAlive(IBattleEntity.Type.AI) > 0)
        {
            foreach (var entity in _entities)
            {
                entity.InitAttack(entity.GetCards());

                var attack = entity.ChooseAttack(entity.GetCards());
                yield return attack.Item1;
                var attackAsCard = attack.Item2.Invoke();

                Debug.Log(attackAsCard.name);

                entity.InitTarget(GetFromType((IBattleEntity.Type)((int)entity.GetEntityType() * -1)));

                var target = entity.ChooseTarget(GetFromType((IBattleEntity.Type)((int)entity.GetEntityType() * -1)));
                yield return target.Item1;
                var targetAsEntity = target.Item2.Invoke();

                Debug.Log(targetAsEntity.GetName());

                targetAsEntity.ReceiveAttack(entity.Attack(attackAsCard, targetAsEntity));
            }
        }
    }

    public void PopulateCards()
    {
        foreach (var card in SaveDataController.Instance.CurrentData.Cards)
        {
            var cardGO = Instantiate(_cardTemplate, _cards.transform);
            cardGO.onClick.AddListener(() => PlayerBattleEntity.SelectCard(card));
        }
    }

    public void PopulateMasks()
    {
        foreach (var mask in SaveDataController.Instance.CurrentData.Masks)
        {
            var maskGO = Instantiate(_maskTemplate, _masks.transform);
            //maskGO.onClick.AddListener(() => PlayerBattleEntity.SelectCard(card));
        }
    }
}
