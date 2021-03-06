﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Promaja : MonoBehaviour
{
    public static Promaja e;

    public static bool IsActive { get; private set; }

    public static float GuestWishModifier => IsActive ? 1.4f : 1f;

    Door[] doors;

    public AudioSource promajaSource;

    float promajaSoundVelo;

    private void Start()
    {
        IsActive = false;
    }

    private void Update()
    {
        promajaSource.volume =
            Mathf.SmoothDamp(promajaSource.volume, IsActive ? 1 : 0, ref promajaSoundVelo, 0.2f);
    }

    private void Awake()
    {
        e = this;
        doors = FindObjectsOfType<Door>();
        //Debug.Log($"Found {doors.Length} doors");
    }

    public static void Refresh()
    {
        int openCount = Prozor.IsProzorOpen ? 1 : 0;
        for (int i = 0; i < e.doors.Length; i++)
        {
            if (e.doors[i].IsOpen)
                openCount++;
        }

        IsActive = openCount >= 2;
        //Debug.LogWarning("PROMAJA IS " + IsActive);
        if (IsActive)
            Debug.Log("Promaja started!");

        if (IsActive)
        {
            // sveca je tajmer, i malo mi cudno da mozes da pauziras tajmer a sve ostalo da radi
            //Candle.e.Extinguish();
        }
    }
}
