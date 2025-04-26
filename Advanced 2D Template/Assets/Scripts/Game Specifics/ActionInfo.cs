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

    public Action action;

    public HealthChangeResult result;
}
