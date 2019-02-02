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
    public GameObject top;
    public GameObject body;
    public GameObject flame;

    [Range(0, 1)]
    public float burnProgress;

    public Vector3 flameStartPosition;
    public Vector3 flameEndPosition;

    protected override void OnStart()
    {
        if (isBurning)
            Ignite();
        else
            Extinguish();
    }

    void Update()
    {
        if (!Application.isPlaying || GameController.I.IsPaused)
            return;
        
        if (isBurning && burnTime > 0)
        {
            burnProgress += Time.deltaTime / burnTime;
            top.transform.localPosition = Vector3.Lerp(flameStartPosition, flameEndPosition, burnProgress);
            body.transform.localScale = new Vector3(1, 1 - burnProgress, 1);
        }

        burnProgress = Mathf.Clamp01(burnProgress);
        
        if (burnProgress >= 1f)
        {
            GameController.I.EndGame();
        }
    }

    public void Ignite()
    {
        if (Promaja.IsActive)
        {
            Debug.LogWarning("AL DUVA PROMAJA");
            return;
        }
        flame.SetActive(true);
        isBurning = true;
    }

    public void Extinguish()
    {
        flame.SetActive(false);
        isBurning = false;
    }
}
