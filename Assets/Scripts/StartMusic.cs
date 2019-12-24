using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMusic : MonoBehaviour
{
    public AudioSource source;
    public AudioSource sourceLoop;

    public AudioClip startClip;

    IEnumerator Start()
    {
        double startDuration = (double)startClip.samples / startClip.frequency;
        yield return new WaitForSeconds(1);
        source.Play();
        sourceLoop.PlayScheduled(AudioSettings.dspTime + startDuration);
    }
}
