using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkContainer : MonoBehaviour
{
    public Drink drinkType;
    public float amount;
    public float maxAmount = 1;
    public float temperature;
    public float coolingSpeed = 0.01f;

    public bool canBeRefilledAtSlavina;

    public bool showAmount = true;
    public bool showTemperature = true;

    public bool TranferDrinkTo(DrinkContainer drinkContainer, float pourAmount)
    {
        if (amount <= 0) return false;

        if (pourAmount <= 0) return false;

        if (drinkContainer.amount > 0 && drinkType != drinkContainer.drinkType)
            return false;

        if (drinkContainer.amount > drinkContainer.maxAmount)
        {
            drinkContainer.amount = drinkContainer.maxAmount;
            return false;
        }

        //Debug.Log("Pouring from: " + name + " to: " + drinkContainer.name);

        drinkContainer.drinkType = drinkType;

        drinkContainer.amount += pourAmount;
        amount -= pourAmount;

        if (amount <= 0) amount = 0;

        drinkContainer.temperature = temperature;

        return true;
    }

    public void AddDrink(Drink _drinkType, float _amount)
    {
        drinkType = _drinkType;
        amount += _amount;
        if (amount > maxAmount) amount = maxAmount;
    }

    public void AddHeat(float heat)
    {
        temperature += heat;
        temperature = Mathf.Clamp01(temperature);
    }

    private void Update()
    {
        temperature -= Time.deltaTime * coolingSpeed;
        temperature = Mathf.Clamp01(temperature);

        if (showAmount && amount > 0)
            DebugUtils.InGameMeter(amount / maxAmount, transform.position, 20, DrinkFoodUtils.GetColor(drinkType));

        if (showTemperature && temperature > 0)
            DebugUtils.InGameMeter(temperature, transform.position, 30, Color.red);
    }

    public void DebugAmount(float offset)
    {

        DebugUtils.Meter(amount / maxAmount, transform.position, offset, DrinkFoodUtils.GetColor(drinkType));
    }
}
