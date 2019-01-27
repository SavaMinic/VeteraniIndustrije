using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prozor : Item
{
    public static bool IsOpen { get; private set; }

    public GameObject openGraphics;
    public GameObject closedGraphics;

    public void Toggle()
    {
        IsOpen = !IsOpen;

        openGraphics.SetActive(IsOpen);
        closedGraphics.SetActive(!IsOpen);
    }
}
