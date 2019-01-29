using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PouringSound : MonoBehaviour
{
    public AudioSource source;

    float targetVolume = 1;

    bool pouredLastFrame = false;
    bool pourThisFrame = false;

    public void Pour()
    {
        pourThisFrame = true;
    }

    private void Update()
    {
        if (pourThisFrame && !pouredLastFrame)
        {
            source.enabled = true;
            source.Play();
        }

        if (!pourThisFrame && pouredLastFrame)
        {
            source.enabled = false;
        }

        pouredLastFrame = pourThisFrame;
        pourThisFrame = false;
    }
}
