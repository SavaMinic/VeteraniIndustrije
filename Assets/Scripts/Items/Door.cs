using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Item
{
    public bool isOpen;

    public GameObject openGraphics;
    public GameObject closedGraphics;

    public void Toggle()
    {
        isOpen = !isOpen;

        openGraphics.SetActive(isOpen);
        closedGraphics.SetActive(!isOpen);
    }
}
