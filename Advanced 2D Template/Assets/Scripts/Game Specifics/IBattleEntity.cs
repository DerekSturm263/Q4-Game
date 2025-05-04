using System.Collections;

public interface IBattleEntity
{
    public enum Type
    {
        Human = 1,
        AI = -1
    }

    public string GetName();

    public Type GetEntityType();

    public IEnumerator DoTurn(BattleController ctx);
    
    public ref Stats GetStats();

    public void Die();
}
