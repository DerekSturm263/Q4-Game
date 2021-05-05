using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    [SerializeField] private int saveTime = 120;

    public static Camera cam;
    public static PlayerMovement player;
    public static UIController uiCont;
    public static EntityAI[] entities;
    public static Pickup[] pickups;
    public static Interactable[] interactables;

    public static GameObject savingIndicator;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        uiCont = FindObjectOfType<UIController>();
        entities = FindObjectsOfType<EntityAI>();
        pickups = FindObjectsOfType<Pickup>();
        interactables = FindObjectsOfType<Interactable>();
        savingIndicator = GameObject.FindGameObjectWithTag("Saving");
        savingIndicator.SetActive(false);

        if (SaveDataController.HasSave())
        {
            LoadGame();
        }
        else
        {
            SaveGame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U) || ((int) Time.realtimeSinceStartup % saveTime == 0 && !savingIndicator.activeSelf))
        {
            SaveGame();
        }
    }

    private static void SaveGame()
    {
        savingIndicator.SetActive(true);

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
        player.breathLeftUnderwater = playerData.underwaterBreathLeft;

        UISaveData uiData = SaveDataController.LoadUI();
        UIController.time = uiData.time;
        UIController.numFood = uiData.berryCount;

        for (int i = 0; i < entities.Length; ++i)
        {
            EntitySaveData entitySaveData = SaveDataController.LoadEntity(i);
            entities[i].transform.position = new Vector2(entitySaveData.position[0], entitySaveData.position[1]);
            entities[i].isSatisfied = entitySaveData.isSatisified;
            entities[i].pickupNum = entitySaveData.itemCarried;

            if (entities[i].pickupNum != 0)
            {
                pickups[entities[i].pickupNum - 1].Grab(entities[i].gameObject);
            }
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
