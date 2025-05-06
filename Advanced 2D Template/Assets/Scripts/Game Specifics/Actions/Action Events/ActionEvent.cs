using System.Collections;
using UnityEngine;

public abstract class ActionEvent : ScriptableObject
{
    public abstract IEnumerator Event(BattleController ctx);
}
