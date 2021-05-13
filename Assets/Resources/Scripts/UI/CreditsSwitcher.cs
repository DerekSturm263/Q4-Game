using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsSwitcher : MonoBehaviour
{
    public Image display;
    public Sprite personal;

    public void DisplayPressedImage()
    {
        display.sprite = personal;
    }
}
