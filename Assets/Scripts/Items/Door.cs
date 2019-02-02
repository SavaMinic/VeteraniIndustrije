using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public static bool IsOpen { get; private set; }

    public GameObject openGraphics;
    public GameObject closedGraphics;
    public GameObject doorCollisionObject;

    public void Toggle()
    {
        IsOpen = !IsOpen;

        openGraphics.SetActive(IsOpen);
        closedGraphics.SetActive(!IsOpen);

        doorCollisionObject.SetActive(!IsOpen);

        Promaja.Refresh();
    }
}
