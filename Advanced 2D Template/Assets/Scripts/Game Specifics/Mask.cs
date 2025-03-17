using UnityEngine;

[CreateAssetMenu(fileName = "New Mask", menuName = "Game/Mask")]
public class Mask : ScriptableObject
{
    [TextArea][SerializeField] private string _description;
}
