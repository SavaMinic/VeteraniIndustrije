using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class GuestWish
{

    public enum GuestWishType
    {
        Random,
        Coffee,
        Rakija,
        Sarma,
        OpenWindow,
        CloseWindow,
        TvPink,
        TvRts,
    }

    public GuestWishType Type;
    public float WaitingTime;
    
    public float RemainingWaitingTime { get; private set; }
    public float Progress => 1f - RemainingWaitingTime / WaitingTime;

    public bool IsActive => RemainingWaitingTime > 0f;
    public bool? IsSuccess { get; private set; }
    public bool IsFinished => IsSuccess.HasValue;

    public GuestWish(GuestWishType type, float waitingTime)
    {
        if (type == GuestWishType.Random)
        {
            type = (GuestWishType)Random.Range(1, Enum.GetValues(typeof(GuestWishType)).Length);
        }
        Type = type;

        WaitingTime = waitingTime;
        Debug.Log("GENERATE " + type + " " +  waitingTime);
    }

    public void ActivateWish()
    {
        RemainingWaitingTime = WaitingTime;
    }

    public void UpdateWish(float dt)
    {
        if (IsFinished || !IsActive)
            return;

        RemainingWaitingTime -= dt;
        if (RemainingWaitingTime <= 0)
        {
            IsSuccess = false;
        }
    }

    public void FinishWish()
    {
        RemainingWaitingTime = 0f;
        IsSuccess = true;
    }
}
