using SingletonBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using Types.Scene;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattlePlayer : BattleEntity
{
    [SerializeField] private GameObject _battleOptions;
    [SerializeField] private Animator _battleOptionsAnim;

    private Func<BattleController, IEnumerator> _currentAction;
    public void SetCurrentAction(Func<BattleController, IEnumerator> action) => _currentAction = action;

    [SerializeField] private int _battleIndex = 0;

    [SerializeField] private List<Button> _actions;

    private BattleEntity _target;
    public void SetTarget(BattleEntity target) => _target = target;

    [SerializeField] private SceneLoadSettings _fleeSceneLoadSettings;

    public void DecrementIndex()
    {
        --_battleIndex;
        if (_battleIndex < 0)
            _battleIndex = 3;

        _battleOptionsAnim.SetInteger("Selected", _battleIndex);
    }

    public void IncrementIndex()
    {
        ++_battleIndex;
        if (_battleIndex > 3)
            _battleIndex = 0;

        _battleOptionsAnim.SetInteger("Selected", _battleIndex);
    }

    public override IEnumerator DoTurn(BattleController ctx)
    {
        _current = this;

        _battleOptions.SetActive(true);

        yield return new WaitUntil(() => _currentAction is not null);
        yield return _currentAction.Invoke(ctx);

        _currentAction = null;
        _target = null;
    }

    public void JumpSelect()
    {
        _anim.SetTrigger("BattleJump");
        Invoke(nameof(SelectAfterJump), 0.5f);
    }

    private void SelectAfterJump()
    {
        _battleOptions.SetActive(false);
        _actions[_battleIndex].onClick.Invoke();
    }

    public void Hop() => _currentAction = HopEnumerator;
    private IEnumerator HopEnumerator(BattleController ctx)
    {
        Debug.Log("Hop");

        EventSystem.current.SetSelectedGameObject(ctx.EnemySpots[0].GetComponentInChildren<Selectable>().gameObject);
        yield return new WaitUntil(() => _target);
    }

    public void Tongue() => _currentAction = TongueEnumerator;
    private IEnumerator TongueEnumerator(BattleController ctx)
    {
        Debug.Log("Tongue");
        
        EventSystem.current.SetSelectedGameObject(ctx.EnemySpots[0].GetComponentInChildren<Selectable>().gameObject);
        yield return new WaitUntil(() => _target);
    }

    public void Item() => _currentAction = ItemEnumerator;
    private IEnumerator ItemEnumerator(BattleController ctx)
    {
        Debug.Log("Item");
        
        EventSystem.current.SetSelectedGameObject(ctx.PlayerSpots[0].GetComponentInChildren<Selectable>().gameObject);
        yield return new WaitUntil(() => _target);
    }

    public void Flee() => _currentAction = FleeEnumerator;
    private IEnumerator FleeEnumerator(BattleController ctx)
    {
        Debug.Log("Flee");

        yield return null;
        SceneController.Instance.Load(_fleeSceneLoadSettings);
    }
}
