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
        ZutiSok,
        CrniSok,
        Water,

        Sarma,

        // tv wishes
        TvOff,
        TvBasketball,
        TvWeather,
        TvFarma,

        // window wishes
        OpenWindow,
        CloseWindow,
    }

    // DON'T FORGET TO PUT AVAILABLE WISHES HERE!
    public readonly IReadOnlyList<GuestWishType> AvailableWishTypes = new List<GuestWishType>
    {
        GuestWishType.Coffee,
        GuestWishType.Rakija,
        GuestWishType.ZutiSok,
        GuestWishType.CrniSok,
        GuestWishType.Water,
        GuestWishType.Sarma,
        GuestWishType.TvOff,
        GuestWishType.TvBasketball,
        GuestWishType.TvWeather,
        GuestWishType.TvFarma,
        GuestWishType.OpenWindow,
        GuestWishType.CloseWindow,
    };

    public GuestWishType Type;
    public float WaitingTime;

    public float RemainingWaitingTime { get; private set; }
    public float Progress => 1f - RemainingWaitingTime / WaitingTime;

    public bool IsActive => RemainingWaitingTime > 0f;
    public bool? IsSuccess { get; private set; }
    public bool IsFinished => IsSuccess.HasValue;

    public bool IsTvWish => Type == GuestWishType.TvOff
                            || Type == GuestWishType.TvBasketball
                            || Type == GuestWishType.TvWeather
                            || Type == GuestWishType.TvFarma;

    public bool IsWindowWish => Type == GuestWishType.OpenWindow
                            || Type == GuestWishType.CloseWindow;

    public bool IsDrinkWish => Type == GuestWishType.Coffee
                            || Type == GuestWishType.Rakija
                            || Type == GuestWishType.ZutiSok
                            || Type == GuestWishType.CrniSok
                            || Type == GuestWishType.Water;

    public bool IsFoodWish => Type == GuestWishType.Sarma;

    public GuestWish(GuestWishType type, float waitingTime)
    {
        if (type == GuestWishType.Random)
        {
            type = AvailableWishTypes[Random.Range(0, AvailableWishTypes.Count)];
            //type = (GuestWishType)Random.Range((int)GuestWishType.TvBasketball, (int)GuestWishType.TvFarma + 1);
            //type = (GuestWishType)Random.Range((int)GuestWishType.OpenWindow, (int)GuestWishType.CloseWindow + 1);
        }
        // check if wish is already fullfillable
        if (type == TV.SelectedChannel
            || (Prozor.IsOpen && type == GuestWishType.OpenWindow)
            || (!Prozor.IsOpen && type == GuestWishType.CloseWindow)
        )
        {
            // pick any other
            var availableWishes = new List<GuestWishType>(AvailableWishTypes);
            availableWishes.Remove(type);
            type = availableWishes[Random.Range(0, availableWishes.Count)];
        }
        Type = type;

        WaitingTime = waitingTime;
        Debug.Log("GENERATE " + type + " " + waitingTime);
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

    public void FinishWish(bool success)
    {
        RemainingWaitingTime = 0f;
        IsSuccess = success;
    }
}
