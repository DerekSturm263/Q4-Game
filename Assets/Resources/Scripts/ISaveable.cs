using System;

public interface ISaveable
{
    public string GetDataPath();

    public float[] SaveFloats();
    public int[] SaveInts();
    public byte[] SaveBytes();
    public bool[] SaveBools();
    public float[,] SaveVector2s();
    public float[,] SaveVector3s();

    public void Load();
}
