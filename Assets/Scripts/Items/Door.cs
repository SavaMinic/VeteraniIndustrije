using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;

public class Door : Interactable, IOpenable
{
    public bool IsOpen { get; private set; }

    public GameObject openGraphics;
    public GameObject closedGraphics;
    public GameObject doorCollisionObject;

    public AudioClip openClip;
    public AudioClip closeClip;

    public AudioMixer mixer;

    public void Toggle()
    {
        IsOpen = !IsOpen;

        openGraphics.SetActive(IsOpen);
        closedGraphics.SetActive(!IsOpen);

        doorCollisionObject.SetActive(!IsOpen);

        Promaja.Refresh();

        if (IsOpen)
        {
            if (openClip) openClip.Play2D(0.5f);
        }
        else
        {
            if (closeClip) closeClip.Play2D(0.5f);
        }

        if (mixer)
        {
            mixer.SetFloat("City Lowpass", IsOpen ? 13000 : 200);
            mixer.SetFloat("City Volume", IsOpen ? 0 : -5);
        }
    }
}
