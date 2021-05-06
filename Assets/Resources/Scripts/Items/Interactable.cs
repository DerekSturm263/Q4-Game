using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] public bool canUse = true;

    public abstract void Effect();
}
