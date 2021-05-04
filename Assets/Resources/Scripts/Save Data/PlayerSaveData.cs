public class PlayerSaveData
{
    public float[] position;
    public int moveState;
    public byte abilities;

    public PlayerSaveData(PlayerMovement player)
    {
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;

        moveState = (int) player.moveState;

        abilities = PlayerMovement.abilities;
    }
}
