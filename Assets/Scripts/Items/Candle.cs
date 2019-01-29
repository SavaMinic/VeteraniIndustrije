using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candle : Interactable
{
    public static Candle e;
    void Awake() { e = this; }

    public float burnTime = 60;

    public bool isBurning;
    public GameObject flame;
    public GameObject body;

    [Range(0, 1)]
    public float burnProgress;

    public Vector3 flameStartPosition;
    public Vector3 flameEndPosition;


    void Update()
    {

        if (isBurning)
        {
            burnProgress += Time.deltaTime / burnTime;
            flame.transform.localPosition = Vector3.Lerp(flameStartPosition, flameEndPosition, burnProgress);
            body.transform.localScale = new Vector3(1, 1 - burnProgress, 1);
        }

        burnProgress = Mathf.Clamp01(burnProgress);
    }

    public void Ignite()
    {
        flame.SetActive(true);
    }
}
