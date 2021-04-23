using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerMovement player;

    private void Awake()
    {
        SaveDataController.Load(player);
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        SaveDataController.Save(player);
    }
}
