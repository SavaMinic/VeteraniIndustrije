﻿using System;
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
    public GameObject smoke;
    public Transform smokeMask;

    [Range(0, 1)]
    public float burnProgress;

    public Vector3 flameStartPosition;
    public Vector3 flameEndPosition;

    public AudioClip igniteClip;

    protected override void OnStart()
    {
        SetMaskScale(0);

        if (isBurning)
            Ignite();
        else
            flame.SetActive(false);
    }

    float timeExtinguished;

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

        if (smoke.activeSelf)
        {
            SetMaskScale((Time.time - timeExtinguished) * 7);
        }
    }

    void SetMaskScale(float y)
    {
        Vector3 maskScale = smokeMask.localScale;
        maskScale.y = Mathf.Clamp(y, 0, 9.682078f);
        smokeMask.localScale = maskScale;
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
        igniteClip.Play2D(1);

        smoke.SetActive(false);
    }

    public void Extinguish()
    {
        flame.SetActive(false);
        isBurning = false;
        smoke.SetActive(true);
        timeExtinguished = Time.time;
    }
}
