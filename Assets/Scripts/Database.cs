using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database e;
    private void Awake() { e = this; }

    public Consumable[] consumables;

    public Consumable[] foods;
    public Consumable[] drinks;

    public GameObject pivoPrefab;
    public GameObject termometarPrefab;

    public Interactable CreatePivo()
    {
        GameObject go = Instantiate(pivoPrefab);
        return go.GetComponent<Pivo>();
    }
}
