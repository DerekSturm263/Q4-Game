public struct AttackInfo
{
    public enum HealthChangeResult
    {
        None,
        Dead
    }

    public IBattleEntity attacker;
    public IBattleEntity defender;

    public float damage;

    public HealthChangeResult result;
}
