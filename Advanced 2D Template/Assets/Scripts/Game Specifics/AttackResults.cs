public struct AttackResults
{
    public enum HealthChangeResult
    {
        None,
        Dead
    }

    public IBattleEntity attacker;
    public IBattleEntity defender;

    public Card card;
    public float damage;

    public HealthChangeResult result;
}
