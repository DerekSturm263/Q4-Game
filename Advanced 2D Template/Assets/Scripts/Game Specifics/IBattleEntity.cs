using System.Collections;
using System.Collections.Generic;

public interface IBattleEntity
{
    public enum Type
    {
        Human = 1,
        AI = -1
    }

    public string GetName();

    public Type GetEntityType();
    public List<Card> GetCards();

    public IEnumerator ChooseAttack(IEnumerable<Card> options);
    public IEnumerator ChooseTarget(IEnumerable<IBattleEntity> options);

    public void Attack(AttackResults attack, IBattleEntity target);

    public void Die();
}
