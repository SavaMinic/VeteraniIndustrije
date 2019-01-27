using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ringla : Slot
{
    public bool isOn;

    public AudioSource vriSource;

    private void Update()
    {
        vriSource.enabled = itemInSlot == true;
    }
}
