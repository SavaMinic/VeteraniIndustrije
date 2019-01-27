using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slavina : Slot
{
    public float fillSpeed = 0.5f;

    public Sprite offSprite;
    public Sprite onSprite;

    public GameObject runningWaterGraphics;

    private void Update()
    {
        if (itemInSlot && itemInSlot.GetComponent<DrinkContainer>())
        {
            DrinkContainer dc = itemInSlot.GetComponent<DrinkContainer>();
            dc.drinkType = Drink.Water;
            dc.amount += Time.deltaTime * fillSpeed;
            if (dc.amount > dc.maxAmount) dc.amount = dc.maxAmount;
        }
    }

    public void ShowMlaz()
    {
        if (runningWaterGraphics)
            runningWaterGraphics.SetActive(true);

        if (offSprite && onSprite)
            GetComponent<SpriteRenderer>().sprite = onSprite;
    }

    public void EndPouringWater()
    {
        if (runningWaterGraphics)
            runningWaterGraphics.SetActive(false);

        if (offSprite && onSprite)
            GetComponent<SpriteRenderer>().sprite = offSprite;
    }
}
