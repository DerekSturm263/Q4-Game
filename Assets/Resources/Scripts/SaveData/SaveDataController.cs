using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveDataController
{
    public const string cameraPath = "/camera.saveData";
    public const string playerPath = "/player.saveData";
    public const string uiPath = "/ui.saveData";
    public const string entityPath = "/entities";
    public const string pickupPath = "/pickups";
    public const string interactablePath = "/interactables";

    public const string extension = ".saveData";

    public static bool HasSave()
    {
        return File.Exists(Application.persistentDataPath + playerPath);
    }

    #region Camera

    public static void SaveCamera()
    {
        try
        {
            string path = Application.persistentDataPath + cameraPath;

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            CameraSaveData data = new CameraSaveData(GameController.cam);

            formatter.Serialize(stream, data);
            stream.Close();

            Debug.Log("Succesfully Saved Camera");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could Not Save Camera\n" + e.Message);
        }
    }

    public static CameraSaveData LoadCamera()
    {
        string path = Application.persistentDataPath + cameraPath;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            CameraSaveData data = formatter.Deserialize(stream) as CameraSaveData;

            stream.Close();

            Debug.Log("Succesfully Loaded Camera");
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    #endregion

    #region Player

    public static void SavePlayer()
    {
        try
        {
            string path = Application.persistentDataPath + playerPath;

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            PlayerSaveData data = new PlayerSaveData(GameController.player);

            formatter.Serialize(stream, data);
            stream.Close();

            Debug.Log("Succesfully Saved Player");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could Not Save Player\n" + e.Message);
        }
    }

    public static PlayerSaveData LoadPlayer()
    {
        string path = Application.persistentDataPath + playerPath;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerSaveData data = formatter.Deserialize(stream) as PlayerSaveData;

            stream.Close();

            Debug.Log("Succesfully Loaded Player");
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    #endregion

    #region UI

    public static void SaveUI()
    {
        try
        {
            string path = Application.persistentDataPath + uiPath;

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            UISaveData data = new UISaveData(GameController.uiCont);

            formatter.Serialize(stream, data);
            stream.Close();

            Debug.Log("Succesfully Saved UI");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could Not Save UI\n" + e.Message);
        }
    }

    public static UISaveData LoadUI()
    {
        string path = Application.persistentDataPath + uiPath;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            UISaveData data = formatter.Deserialize(stream) as UISaveData;

            stream.Close();

            Debug.Log("Succesfully Loaded UI");
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    #endregion

    #region Entities

    public static void SaveEntity(int i)
    {
        try
        {
            string path = Application.persistentDataPath + entityPath + i + extension;

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            EntitySaveData data = new EntitySaveData(GameController.entities[i]);

            formatter.Serialize(stream, data);
            stream.Close();

            Debug.Log("Succesfully Saved Entity " + i);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could Not Save Entity\n" + e.Message);
        }
    }

    public static EntitySaveData LoadEntity(int i)
    {
        string path = Application.persistentDataPath + entityPath + i + extension;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            EntitySaveData data = formatter.Deserialize(stream) as EntitySaveData;

            stream.Close();

            Debug.Log("Succesfully Loaded Entity " + i);
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    #endregion

    #region Pickups

    public static void SavePickup(int i)
    {
        try
        {
            string path = Application.persistentDataPath + pickupPath + i + extension;

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            PickupSaveData data = new PickupSaveData(GameController.pickups[i]);

            formatter.Serialize(stream, data);
            stream.Close();

            Debug.Log("Succesfully Saved Pickup " + i);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could Not Save Pickup\n" + e.Message);
        }
    }

    public static PickupSaveData LoadPickup(int i)
    {
        string path = Application.persistentDataPath + pickupPath + i + extension;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PickupSaveData data = formatter.Deserialize(stream) as PickupSaveData;

            stream.Close();

            Debug.Log("Succesfully Loaded Pickup " + i);
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    #endregion

    #region Interactables

    public static void SaveInteractable(int i)
    {
        try
        {
            string path = Application.persistentDataPath + interactablePath + i + extension;

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            InteractableSaveData data = new InteractableSaveData(GameController.interactables[i]);

            formatter.Serialize(stream, data);
            stream.Close();

            Debug.Log("Succesfully Saved Interactable " + i);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could Not Save Interactable\n" + e.Message);
        }
    }

    public static InteractableSaveData LoadInteractable(int i)
    {
        string path = Application.persistentDataPath + interactablePath + i + extension;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            InteractableSaveData data = formatter.Deserialize(stream) as InteractableSaveData;

            stream.Close();

            Debug.Log("Succesfully Loaded Interactable " + i);
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    #endregion
}
