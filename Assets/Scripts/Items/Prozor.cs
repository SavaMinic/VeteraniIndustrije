using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOpenable
{
    bool IsOpen { get; }
}

public class Prozor : Interactable, IOpenable
{
    public static bool IsProzorOpen { get; private set; }
    public bool IsOpen => IsProzorOpen;

    public GameObject openGraphics;
    public GameObject closedGraphics;

    public void Toggle()
    {
        IsProzorOpen = !IsProzorOpen;

        openGraphics.SetActive(IsProzorOpen);
        closedGraphics.SetActive(!IsProzorOpen);

        Promaja.Refresh();
    }
}
