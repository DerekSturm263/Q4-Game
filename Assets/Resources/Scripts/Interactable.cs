using UnityEngine;
using System;

public abstract class Interactable : MonoBehaviour
{
    public bool canUse = false;

    public abstract void Effect();
}
