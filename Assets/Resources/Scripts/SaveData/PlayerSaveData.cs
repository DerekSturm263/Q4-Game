[System.Serializable]
public class PlayerSaveData
{
    public float[] position = new float[2];
    public int moveState;
    public byte abilities;
    public float underwaterBreathLeft;

    public float[] lastPosBeforeSwimOrPit = new float[2];
    public float[] lastPosBeforeCaughtByEnemy = new float[2];
    public float[] lastPosBeforeFoodRunsOut = new float[2];
    public float[] lastPosBeforeThorns = new float[2];

    public PlayerSaveData(PlayerMovement player)
    {
        this.position[0] = player.transform.position.x;
        this.position[1] = player.transform.position.y;

        this.moveState = (int) player.moveState;

        this.abilities = PlayerMovement.abilities;

        this.underwaterBreathLeft = player.breathLeftUnderwater;

        this.lastPosBeforeSwimOrPit[0] = PlayerMovement.lastPosBeforeSwimOrPit.x;
        this.lastPosBeforeSwimOrPit[1] = PlayerMovement.lastPosBeforeSwimOrPit.y;
        this.lastPosBeforeCaughtByEnemy[0] = PlayerMovement.lastPosBeforeCaughtByEnemy.x;
        this.lastPosBeforeCaughtByEnemy[1] = PlayerMovement.lastPosBeforeCaughtByEnemy.y;
        this.lastPosBeforeFoodRunsOut[0] = PlayerMovement.lastPosBeforeFoodRunsOut.x;
        this.lastPosBeforeFoodRunsOut[1] = PlayerMovement.lastPosBeforeFoodRunsOut.y;
        this.lastPosBeforeThorns[0] = PlayerMovement.lastPosBeforeThorns.x;
        this.lastPosBeforeThorns[1] = PlayerMovement.lastPosBeforeThorns.y;
    }
}
