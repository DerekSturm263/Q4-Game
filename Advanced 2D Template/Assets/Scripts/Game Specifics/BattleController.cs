using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleController : Types.SingletonBehaviour<BattleController>
{
    [SerializeField] private PlayerBattleEntity _playerTemplate;
    [SerializeField] private EnemyBattleEntity _enemyTemplate;

    private IEnumerable<IBattleEntity> _entities;

    private IEnumerable<IBattleEntity> GetFromType(IBattleEntity.Type type) => _entities.Where(item => item.GetEntityType() == type);
    private int GetNumberAlive(IBattleEntity.Type type) => GetFromType(type).Count();

    private void Start()
    {
        StartBattle(SingletonBehaviours.SceneController.Instance.GetSceneParameter<BattleSetup>("Setup"));
    }

    public void StartBattle(BattleSetupAsset setup) => StartBattle(setup.Value);

    public void StartBattle(BattleSetup setup)
    {
        foreach ()
        {

        }

        StartCoroutine(Battle(setup));
    }

    IEnumerator Battle(BattleSetup setup)
    {
        _entities = setup.Entities;

        while (GetNumberAlive(IBattleEntity.Type.Human) > 0 || GetNumberAlive(IBattleEntity.Type.AI) > 0)
        {
            foreach (var entity in _entities)
            {
                var attack = entity.ChooseAttack(entity.GetCards());
                yield return attack;

                Debug.Log(((AttackResults)attack.Current).card.name);

                var target = entity.ChooseTarget(GetFromType((IBattleEntity.Type)((int)entity.GetEntityType() * -1)));
                yield return target;

                Debug.Log((target.Current as IBattleEntity).GetName());

                entity.Attack((AttackResults)attack.Current, target.Current as IBattleEntity);
            }
        }
    }
}
