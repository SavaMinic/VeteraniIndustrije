using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database e;
    private void Awake() { e = this; }

    private void Start()
    {
        if (e == null)
            e = this;
    }

    public Consumable[] consumables;

    public GameObject pivoPrefab;
    public GameObject termometarPrefab;
    public RectTransform containerMeterParent;
    public GameObject containerMeterPrefab;
    public GameObject flekaPrefab;

    public AudioClip[] stepClips;
    public AudioClip[] broomSweepClips;

    public Door entranceDoor;
    public Door balkonDoor;

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

    public void CreateContainerMeter(Container container)
    {
        GameObject ago = Instantiate(containerMeterPrefab);
        ago.transform.SetParent(containerMeterParent);
        ago.GetComponent<ContainerMeter>().container = container;
    }
}
