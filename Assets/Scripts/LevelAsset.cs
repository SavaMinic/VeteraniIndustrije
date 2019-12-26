using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelAsset")]
public class LevelAsset : ScriptableObject
{
    public int CandleDuration;
    public int LimitGuestNumber;
    public float DelayForGeneratingGuestsSlow = 10f;
    public float DelayForGeneratingGuestsFast = 25f;
    public List<GuestWish.GuestWishType> AvailableWishes;
    public bool IsRaining;

    public string Name => name;
}
