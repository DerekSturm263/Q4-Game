[System.Serializable]
public class PlayerSaveData
{
    public float[] position = new float[2];
    public int moveState;
    public byte abilities;

    public PlayerSaveData(PlayerMovement player)
    {
        this.position[0] = player.transform.position.x;
        this.position[1] = player.transform.position.y;

        this.moveState = (int) player.moveState;

        this.abilities = PlayerMovement.abilities;
    }
}
