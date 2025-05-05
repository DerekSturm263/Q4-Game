using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public abstract class BattleEntity : Selectable, IBattleEntity, ISubmitHandler, IPointerDownHandler
{
    protected static BattlePlayer _current;

    [SerializeField] private IBattleEntity.Type _type;

    protected Animator _anim;
    public Animator Animator => _anim;

    [SerializeField] private EntityStats _stats;

    [SerializeField] protected DisplayHealth _healthDisplay;

    protected Func<BattleController, IEnumerator> _currentAction;
    public Func<BattleController, IEnumerator> CurrentAction => _currentAction;

    protected BattleEntity _target;
    public BattleEntity Target => _target;
    public void SetTarget(BattleEntity target) => _target = target;

    protected Stats _currentStats;
    public ref Stats GetStats() => ref _currentStats;
    public void SetStats(Stats stats) => _currentStats = stats;

    protected bool _isEvading;

    public void TakeDamage(float damage)
    {
        if (_isEvading)
            return;

        if (GetStats().ModifyHealth(-damage) == ActionInfo.HealthChangeResult.Dead)
        {
            Die();
        }

        if (_healthDisplay)
            _healthDisplay.UpdateDisplay(_currentStats.CurrentHealth / _currentStats.MaxHealth);
    }

    protected override void Awake()
    {
        _anim = GetComponent<Animator>();

        if (_stats)
            _currentStats = _stats.Stats;
    }

    private void Update()
    {
        _anim.SetBool("IsDead", !_currentStats.IsAlive);
    }

    public string GetName() => name;

    public IBattleEntity.Type GetEntityType() => _type;

    public abstract IEnumerator DoTurn(BattleController ctx);

    public void Die()
    {
        gameObject.SetActive(false);
    }

    public void OnSubmit(BaseEventData eventData) => DoSelect(eventData);
    public override void OnPointerDown(PointerEventData eventData) => DoSelect(eventData);

    public void DoSelect(BaseEventData eventData)
    {
        _current.SetTarget(this);
        Debug.Log($"{name} selected as target");
    }
}
