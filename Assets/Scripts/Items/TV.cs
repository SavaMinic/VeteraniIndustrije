﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV : Interactable
{
    public int channel;

    public GameObject[] channelSprites;

    public float switchVolume = 1;
    public AudioClip switchClip;
    public AudioClip switchOffClip;

    private GuestWish.GuestWishType[] channelTypes =
    {
        GuestWish.GuestWishType.TvOff,
        GuestWish.GuestWishType.TvBasketball,
        GuestWish.GuestWishType.TvWeather,
        GuestWish.GuestWishType.TvFarma,
        GuestWish.GuestWishType.TvMuzika
    };

    // there is only one tv, no need to get references
    public static GuestWish.GuestWishType SelectedChannel { get; private set; }

    protected override void OnStart()
    {
        SetChannel();
    }

    public void SwitchChannel()
    {
        channel++;

        if (channel >= channelSprites.Length)
            channel = 0;

        SetChannel();

        if (channel == 0)
            switchOffClip.Play2D(switchVolume);
        else
            switchClip.Play2D(switchVolume);
    }

    void SetChannel()
    {
        for (int i = 0; i < channelSprites.Length; i++)
            channelSprites[i].SetActive(false);

        channelSprites[channel].SetActive(true);
        SelectedChannel = channelTypes[channel];
    }
}
