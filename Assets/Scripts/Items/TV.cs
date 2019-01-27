using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV : Item
{
    public int channel;

    public GameObject[] channelSprites;
    public GuestWish.GuestWishType[] channelTypes;

    private void Start()
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
    }
}
