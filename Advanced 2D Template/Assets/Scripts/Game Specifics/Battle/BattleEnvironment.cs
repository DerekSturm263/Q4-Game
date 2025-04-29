using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Battle Environment", menuName = "Game/Battle Environment")]
public class BattleEnvironment : Asset
{
    [SerializeField] private UnityEvent<BattleController> _onTurnStart;
    [SerializeField] private UnityEvent<BattleController> _onTurnEnd;
}
