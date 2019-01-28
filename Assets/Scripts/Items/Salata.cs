using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salata : Interactable
{
    Container foodContainer;

    public int choppingAmount = 0;
    public int maxChopping = 10;
    public bool IsChoppedAndReady { get { return choppingAmount >= maxChopping; } }

    public void Chop()
    {
        choppingAmount++;
    }
}
