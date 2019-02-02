﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Promaja
{
    public static bool IsActive { get; private set; }

    public static float GuestWishModifier => IsActive ? 1.4f : 1f;
    
    public static void Refresh()
    {
        IsActive = Door.IsOpen && Prozor.IsOpen;
        Debug.LogWarning("PROMAJA IS " + IsActive);

        if (IsActive)
        {
            // sveca je tajmer, i malo mi cudno da mozes da pauziras tajmer a sve ostalo da radi
            //Candle.e.Extinguish();
        }
    }
}
