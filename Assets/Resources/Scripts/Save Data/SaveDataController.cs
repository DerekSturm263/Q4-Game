using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveDataController
{
    public const string cameraPath = "/camera.save";
    public const string playerPath = "/player.save";
    public const string entityPath = "/entities.save";
    public const string pickupPath = "/pickups.save";
    public const string interactablePath = "/interactables.save";

    #region Camera

    public static void SaveCamera(Camera cam)
    {
        string path = Application.persistentDataPath + cameraPath;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        CameraSaveData data = new CameraSaveData(cam);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static Camera LoadCamera()
    {
        string path = Application.persistentDataPath + cameraPath;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Camera data = formatter.Deserialize(stream) as Camera;

            stream.Close();
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

    public static void SavePlayer(PlayerMovement player)
    {
        string path = Application.persistentDataPath + playerPath;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerSaveData data = new PlayerSaveData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerMovement LoadPlayer()
    {
        string path = Application.persistentDataPath + playerPath;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerMovement data = formatter.Deserialize(stream) as PlayerMovement;

            stream.Close();
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

    public static void SaveEntity(EntityAI entity)
    {
        string path = Application.persistentDataPath + entityPath;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        EntitySaveData data = new EntitySaveData(entity);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static EntityAI LoadEntity()
    {
        string path = Application.persistentDataPath + entityPath;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            EntityAI data = formatter.Deserialize(stream) as EntityAI;

            stream.Close();
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

    public static void SavePickup(Pickup pickup)
    {
        string path = Application.persistentDataPath + pickupPath;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        PickupSaveData data = new PickupSaveData(pickup);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static Pickup LoadPickup()
    {
        return null;
    }

    #endregion

    #region Interactables

    public static void SaveInteractable(Interactable interactable)
    {
        string path = Application.persistentDataPath + interactablePath;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        InteractableSaveData data = new InteractableSaveData(interactable);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static Interactable LoadInteractable()
    {
        return null;
    }

    #endregion
}
