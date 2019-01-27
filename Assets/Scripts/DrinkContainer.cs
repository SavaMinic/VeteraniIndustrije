﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkContainer : MonoBehaviour
{
    public Drink drinkType;
    public float amount;
    public float maxAmount = 1;
    public float temperature;

    public float coolingSpeed = 0.01f;

    public bool TranferDrinkTo(DrinkContainer drinkContainer, float amount)
    {
        if (amount <= 0) return false;

        if (amount > 0 && drinkType != drinkContainer.drinkType)
            return false;

        if (drinkContainer.amount > drinkContainer.maxAmount)
        {
            drinkContainer.amount = drinkContainer.maxAmount;
            return false;
        }

        drinkContainer.drinkType = drinkType;

        drinkContainer.amount += amount;
        amount -= amount;

        drinkContainer.temperature = temperature;

        return true;
    }

    private void Update()
    {
        temperature -= Time.deltaTime * coolingSpeed;
        temperature = Mathf.Clamp01(temperature);
    }

    public void DebugAmount(float offset)
    {
        DebugUtils.Meter(amount / maxAmount, transform.position, offset, DrinkFoodUtils.GetColor(drinkType));
    }
}
