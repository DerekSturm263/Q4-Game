[System.Serializable]
public class BubbleSaveData
{
    public float timeSincePopped;

    public BubbleSaveData(AirBubble bubble)
    {
        timeSincePopped = bubble.timeSincePopped;
    }
}
