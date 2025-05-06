using System;

public struct ActionInfo
{
    public enum HealthChangeResult
    {
        None,
        Dead
    }

    public IBattleEntity user;
    public IBattleEntity target;

    public BattleAction action;

    public HealthChangeResult result;
}
