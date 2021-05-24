using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveDataController
{
    public const string Extension = ".data";

    public const string AutoSavePath = "/Auto Save";
    public const string ManualSavePath = "/Manual Save";

    public const string CameraPath = "/camera" + Extension;
    public const string PlayerPath = "/player" + Extension;
    public const string UIPath = "/ui" + Extension;
    public const string EntityPath = "/entity";
    public const string PickupPath = "/pickup";
    public const string InteractablePath = "/interactable";
    public const string BubblePath = "/bubble";
    public const string CutscenePath = "/cutscene";
    public const string TutorialPath = "/tutorial";

    public static bool HasSave()
    {
        return File.Exists(Application.persistentDataPath + AutoSavePath + PlayerPath) && File.Exists(Application.persistentDataPath + ManualSavePath + PlayerPath);
    }

    #region Camera

    public static void SaveCamera(string filePath)
    {
        try
        {
            string path = filePath + CameraPath;

            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
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

    public static CameraSaveData LoadCamera(string filePath)
    {
        string path = filePath + CameraPath;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
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

    public static void SavePlayer(string filePath)
    {
        try
        {
            string path = filePath + PlayerPath;

            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
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

    public static PlayerSaveData LoadPlayer(string filePath)
    {
        string path = filePath + PlayerPath;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
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

    public static void SaveUI(string filePath)
    {
        try
        {
            string path = filePath + UIPath;

            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
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

    public static UISaveData LoadUI(string filePath)
    {
        string path = filePath + UIPath;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
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

    public static void SaveEntity(int i, string filePath)
    {
        try
        {
            string path = filePath + EntityPath + i + Extension;

            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
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

    public static EntitySaveData LoadEntity(int i, string filePath)
    {
        string path = filePath + EntityPath + i + Extension;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
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

    public static void SavePickup(int i, string filePath)
    {
        try
        {
            string path = filePath + PickupPath + i + Extension;

            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
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

    public static PickupSaveData LoadPickup(int i, string filePath)
    {
        string path = filePath + PickupPath + i + Extension;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
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

    public static void SaveInteractable(int i, string filePath)
    {
        try
        {
            string path = filePath + InteractablePath + i + Extension;

            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
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

    public static InteractableSaveData LoadInteractable(int i, string filePath)
    {
        string path = filePath + InteractablePath + i + Extension;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
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

    #region Bubbles

    public static void SaveBubble(int i, string filePath)
    {
        try
        {
            string path = filePath + BubblePath + i + Extension;

            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
            FileStream stream = new FileStream(path, FileMode.Create);
            BubbleSaveData data = new BubbleSaveData(GameController.bubbles[i]);

            formatter.Serialize(stream, data);
            stream.Close();

            Debug.Log("Succesfully Saved Bubble " + i);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could Not Save Bubble\n" + e.Message);
        }
    }

    public static BubbleSaveData LoadBubble(int i, string filePath)
    {
        string path = filePath + BubblePath + i + Extension;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
            FileStream stream = new FileStream(path, FileMode.Open);
            BubbleSaveData data = formatter.Deserialize(stream) as BubbleSaveData;

            stream.Close();

            Debug.Log("Succesfully Loaded Bubble " + i);
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    #endregion

    #region Cutscenes

    public static void SaveCutscene(int i, string filePath)
    {
        try
        {
            string path = filePath + CutscenePath + i + Extension;

            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
            FileStream stream = new FileStream(path, FileMode.Create);
            CutsceneSaveData data = new CutsceneSaveData(GameController.cutscenes[i]);

            formatter.Serialize(stream, data);
            stream.Close();

            Debug.Log("Succesfully Saved Cutscene " + i);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could Not Save Cutscene\n" + e.Message);
        }
    }

    public static CutsceneSaveData LoadCutscene(int i, string filePath)
    {
        string path = filePath + CutscenePath + i + Extension;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
            FileStream stream = new FileStream(path, FileMode.Open);
            CutsceneSaveData data = formatter.Deserialize(stream) as CutsceneSaveData;

            stream.Close();

            Debug.Log("Succesfully Loaded Cutscene " + i);
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    #endregion

    #region Tutorials

    public static void SaveTutorial(int i, string filePath)
    {
        try
        {
            string path = filePath + TutorialPath + i + Extension;

            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
            FileStream stream = new FileStream(path, FileMode.Create);
            TutorialSaveData data = new TutorialSaveData(GameController.tutorials[i]);

            formatter.Serialize(stream, data);
            stream.Close();

            Debug.Log("Succesfully Saved Tutorial " + i);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could Not Save Tutorial\n" + e.Message);
        }
    }

    public static TutorialSaveData LoadTutorial(int i, string filePath)
    {
        string path = filePath + TutorialPath + i + Extension;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(filePath);
            FileStream stream = new FileStream(path, FileMode.Open);
            TutorialSaveData data = formatter.Deserialize(stream) as TutorialSaveData;

            stream.Close();

            Debug.Log("Succesfully Loaded Tutorial " + i);
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
