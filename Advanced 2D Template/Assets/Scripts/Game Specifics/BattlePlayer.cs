using SingletonBehaviours;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattlePlayer : BattleEntity
{
    private static bool _canEvade;
    public static void SetCanEvade(bool canEvade) => _canEvade = canEvade;

    [SerializeField] private GameObject _battleOptions;
    [SerializeField] private Animator _battleOptionsAnim;

    [SerializeField] private int _battleIndex = 0;

    [SerializeField] private List<Button> _actions;

    private bool _isJumping;

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

    protected override void Awake()
    {
        base.Awake();

        _healthDisplay = FindFirstObjectByType<DisplayHealth>();
    }

    public override IEnumerator DoTurn(BattleController ctx)
    {
        _current = this;

        _battleOptions.SetActive(true);

        _currentAction = null;
        _target = null;
        EventSystem.current.SetSelectedGameObject(null);

        yield return new WaitUntil(() => _currentAction is not null);
        yield return _currentAction.Invoke(ctx);

        _currentAction = null;
        _target = null;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void JumpSelect()
    {
        _anim.SetTrigger("Select");
        Invoke(nameof(SelectAfterJump), 0.5f);
    }

    private void SelectAfterJump()
    {
        _battleOptions.SetActive(false);
        _actions[_battleIndex].onClick.Invoke();
    }

    public void Jump()
    {
        if (_isJumping || !_canEvade)
            return;

        StartCoroutine(JumpEnumerator());
    }

    private IEnumerator JumpEnumerator()
    {
        _isJumping = true;

        Vector3 originalPos2 = transform.position;
        for (float t = 0; t < 1f; t += Time.deltaTime * 1.5f)
        {
            if (t > 0.2f)
                _isEvading = true;

            if (t > 0.8f)
                _isEvading = false;

            Vector2 position = new
            (
                originalPos2.x,
                originalPos2.y + Mathf.Sin(t * Mathf.PI) * 1.75f
            );
            transform.position = position;

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.35f);

        _isJumping = false;
    }

    public void Hop() => _currentAction = _currentStats.Actions[0].DoAction;
    public void Tongue() => _currentAction = _currentStats.Actions[1].DoAction;

    public void Item() => _currentAction = ItemEnumerator;
    private IEnumerator ItemEnumerator(BattleController ctx)
    {
        Debug.Log("Item");

        EventSystem.current.SetSelectedGameObject((ctx.GetFromType(IBattleEntity.Type.Human).ElementAt(0) as BattleEntity).gameObject);
        yield return new WaitUntil(() => _target);
    }

    public void Flee() => _currentAction = FleeEnumerator;
    private IEnumerator FleeEnumerator(BattleController ctx)
    {
        Debug.Log("Flee");

        yield return null;
        SceneController.Instance.Load(ctx.BackToOverworld);
    }
}
