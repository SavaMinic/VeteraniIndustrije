using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KalabukaAudio : MonoBehaviour
{
    AudioSource _source;
    AudioSource source { get { if (!_source) _source = GetComponent<AudioSource>(); return _source; } }
    
    void Update()
    {
        float targetVolume = GuestManager.I.GuestCountRatio;

        source.volume = Mathf.Lerp(source.volume, targetVolume, Time.deltaTime);

        Debug.Log(source.volume);
    }
}
