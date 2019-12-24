using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motorola : MonoBehaviour
{
    public AudioClip[] motorolaClips;
    Coroutine coroutine;

    private void Start()
    {
        coroutine = StartCoroutine(PlayRandom());
    }

    IEnumerator PlayRandom()
    {
        while (true)
        {
            motorolaClips.Play2D(true, 0.2f);
            yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(coroutine);
    }
}
