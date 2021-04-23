using UnityEngine;

public class SaveData
{
    public string dataPath;

    public float[] saveFloats;
    public int[] saveInts;
    public byte[] saveBytes;
    public bool[] saveBools;
    public float[,] saveVector2s;
    public float[,] saveVector3s;

    public SaveData(ISaveable saveObject)
    {
        dataPath = saveObject.GetDataPath();

        saveFloats = saveObject.SaveFloats();
        saveInts = saveObject.SaveInts();
        saveBytes = saveObject.SaveBytes();
        saveBools = saveObject.SaveBools();
        saveVector2s = saveObject.SaveVector2s();
        saveVector3s = saveObject.SaveVector3s();
    }
}
