using UnityEngine;

public interface IBattleEntity
{
    public enum Type
    {
        Human = 1,
        AI = -1
    }

    public string GetName();

    public Type GetEntityType();

    public void InitAction(BattleController ctx);
    public (CustomYieldInstruction, ActionInfo) ChooseAction(BattleController ctx);
    
    public ref Stats GetStats();

    public void Die();
}
