using System;

public interface ISaveable
{
    string GetDataPath();

    float[] SaveFloats();
    int[] SaveInts();
    byte[] SaveBytes();
    bool[] SaveBools();
    float[,] SaveVector2s();
    float[,] SaveVector3s();

    void Load();
}
