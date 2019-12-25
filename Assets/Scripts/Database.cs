using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database e;
    private void Awake() { e = this; }

    public Consumable[] consumables;

    public GameObject pivoPrefab;
    public GameObject termometarPrefab;
    public GameObject flekaPrefab;

    public AudioClip[] stepClips;
    public AudioClip[] broomSweepClips;

    public Door entranceDoor;

    public Interactable CreatePivo()
    {
        GameObject go = Instantiate(pivoPrefab);
        return go.GetComponent<Pivo>();
    }

    public Fleka CreateFleka(Vector3 position)
    {
        var flekaObject = Instantiate(flekaPrefab);
        position.y = -1;
        flekaObject.transform.position = position;
        return flekaObject.GetComponent<Fleka>();
    }
}
