using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    [SerializeField] private float saveTime = 60f;

    public static Camera cam;
    public static PlayerMovement player;
    public static UIController uiCont;
    public static EntityAI[] entities;
    public static Pickup[] pickups;
    public static Interactable[] interactables;

    // TODO: Add code for UI stuff from Jared.

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        uiCont = FindObjectOfType<UIController>();
        entities = FindObjectsOfType<EntityAI>();
        pickups = FindObjectsOfType<Pickup>();
        interactables = FindObjectsOfType<Interactable>();

        if (SaveDataController.HasSave())
        {
            LoadGame();
        }
        else
        {
            SaveGame();
        }

        InvokeRepeating("SaveGame", saveTime, saveTime);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            SaveGame();
        }
    }

    private static void SaveGame()
    {
        SaveDataController.SaveCamera();
        SaveDataController.SavePlayer();
        SaveDataController.SaveUI();

        for (int i = 0; i < entities.Length; ++i)
        {
            SaveDataController.SaveEntity(i);
        }

        for (int i = 0; i < pickups.Length; ++i)
        {
            SaveDataController.SavePickup(i);
        }

        for (int i = 0; i < interactables.Length; ++i)
        {
            SaveDataController.SaveInteractable(i);
        }
    }

    private static void LoadGame()
    {
        CameraSaveData camData = SaveDataController.LoadCamera();
        cam.transform.position = new Vector2(camData.position[0], camData.position[1]);

        PlayerSaveData playerData = SaveDataController.LoadPlayer();
        player.transform.position = new Vector2(playerData.position[0], playerData.position[1]);
        player.moveState = (PlayerMovement.MoveState) playerData.moveState;
        PlayerMovement.abilities = playerData.abilities;

        UISaveData uiData = SaveDataController.LoadUI();
        UIController.time = uiData.time;
        UIController.numFood = uiData.berryCount;

        for (int i = 0; i < entities.Length; ++i)
        {
            EntitySaveData entitySaveData = SaveDataController.LoadEntity(i);
            entities[i].transform.position = new Vector2(entitySaveData.position[0], entitySaveData.position[1]);
            entities[i].isSatisfied = entitySaveData.isSatisified;
        }

        for (int i = 0; i < pickups.Length; ++i)
        {
            PickupSaveData pickupSaveData = SaveDataController.LoadPickup(i);
            pickups[i].transform.position = new Vector2(pickupSaveData.position[0], pickupSaveData.position[1]);
        }

        for (int i = 0; i < interactables.Length; ++i)
        {
            InteractableSaveData interactableSaveData = SaveDataController.LoadInteractable(i);
            interactables[i].canUse = interactableSaveData.canUse;
        }
    }
}
