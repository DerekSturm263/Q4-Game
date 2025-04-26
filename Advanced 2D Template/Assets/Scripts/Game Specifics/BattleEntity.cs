using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public abstract class BattleEntity : Selectable, IBattleEntity, ISubmitHandler
{
    [SerializeField] private IBattleEntity.Type _type;

    private Animator _anim;

    [SerializeField] private EntityStats _stats;

    private Stats _currentStats;
    public virtual ref Stats GetStats() => ref _currentStats;
    public void SetStats(Stats stats) => _currentStats = stats;

    protected override void Awake()
    {
        _anim = GetComponent<Animator>();

        if (_stats)
            _currentStats = _stats.Stats;
    }

    public string GetName() => name;

    public IBattleEntity.Type GetEntityType() => _type;

    public abstract void InitAction(BattleController ctx);
    public abstract (CustomYieldInstruction, ActionInfo) ChooseAction(BattleController ctx);

    public abstract void OnSubmit(BaseEventData eventData);

    public void Die()
    {
        _anim.SetTrigger("Die");
    }
}
