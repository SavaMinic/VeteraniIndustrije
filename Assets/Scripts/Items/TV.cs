using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV : Item
{
    public int channel;

    public GameObject[] channelSprites;

    private GuestWish.GuestWishType[] channelTypes =
    {
        GuestWish.GuestWishType.TvBasketball,
        GuestWish.GuestWishType.TvWeather,
        GuestWish.GuestWishType.TvFarma,
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
    }

    void SetChannel()
    {
        for (int i = 0; i < channelSprites.Length; i++)
            channelSprites[i].SetActive(false);

        channelSprites[channel].SetActive(true);
        SelectedChannel = channelTypes[channel];
    }
}
