using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public static Slot[] all;

    public Item itemInSlot;

    private void Start()
    {
        if (all == null)
            all = FindObjectsOfType<Slot>();
    }
}
