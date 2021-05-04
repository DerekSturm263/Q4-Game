using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    [SerializeField] private float saveTime = 60f;

    private static Camera cam;
    private static PlayerMovement player;
    private static EntityAI[] entities;
    private static Pickup[] pickups;
    private static Interactable[] interactables;

    // TODO: Add code for UI stuff from Jared.

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        cam = FindObjectOfType<Camera>();
        player = FindObjectOfType<PlayerMovement>();
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

    private void OnApplicationQuit()
    {
        SaveGame(); // Remove once save button is added.
    }

    private static void LoadGame()
    {
        try
        {
            cam = SaveDataController.LoadCamera();
            player = SaveDataController.LoadPlayer();

            //for (int i = 0; i < entities.Length; ++i)
            //{
            //    entities[i] = SaveDataController.LoadEntity();
            //}

            //for (int i = 0; i < pickups.Length; ++i)
            //{
            //    pickups[i] = SaveDataController.LoadPickup();
            //}

            //for (int i = 0; i < interactables.Length; ++i)
            //{
            //    interactables[i] = SaveDataController.LoadInteractable();
            //}

            Debug.Log("Game Loaded Succesfully!");
        }
        catch (Exception e)
        {
            Debug.LogError("Game Could Not Be Loaded to " + Application.persistentDataPath + "\n" + e.Message);
        }
    }

    private static void SaveGame()
    {
        try
        {
            SaveDataController.SaveCamera(cam);
            SaveDataController.SavePlayer(player);

            //for (int i = 0; i < entities.Length; ++i)
            //{
            //    SaveDataController.SaveEntity(entities[i]);
            //}

            //for (int i = 0; i < pickups.Length; ++i)
            //{
            //    SaveDataController.SavePickup(pickups[i]);
            //}

            //for (int i = 0; i < interactables.Length; ++i)
            //{
            //    SaveDataController.SaveInteractable(interactables[i]);
            //}

            Debug.Log("Game Saved Succesfully!");
        }
        catch (Exception e)
        {
            Debug.LogError("Game Could Not Be Saved to " + Application.persistentDataPath + "\n" + e.Message);
        }
    }
}
