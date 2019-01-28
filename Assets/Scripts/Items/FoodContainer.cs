using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
public class FoodContainer : MonoBehaviour
{
    public Food foodType;
    public float amount;
    public float maxAmount = 1;
    public float temperature;

    public bool showAmount = true;
    public bool showTemperature = true;

    public float coolingSpeed = 0.01f;

    public bool TransferFoodTo(FoodContainer foodContainer, float pourAmount)
    {
        if (amount <= 0) return false;

        if (pourAmount <= 0) return false;

        if (foodContainer.amount > 0 && foodType != foodContainer.foodType)
            return false;

        if (foodContainer.amount > foodContainer.maxAmount)
        {
            foodContainer.amount = foodContainer.maxAmount;
            return false;
        }

        //Debug.Log("Pouring from: " + name + " to: " + drinkContainer.name);

        foodContainer.foodType = foodType;

        foodContainer.amount += pourAmount;
        amount -= pourAmount;

        if (amount <= 0) amount = 0;

        foodContainer.temperature = temperature;

        return true;
    }

    public void AddFood(Food _foodType, float _amount)
    {
        foodType = _foodType;
        amount += _amount;
        amount = Mathf.Clamp(amount, 0, maxAmount);
    }

    public void AddHeat(float heat)
    {
        temperature += heat;
        temperature = Mathf.Clamp01(temperature);
    }

    private void Update()
    {
        // constant cooling
        temperature -= Time.deltaTime * coolingSpeed;

        // clamp
        amount = Mathf.Clamp(amount, 0, maxAmount);
        temperature = Mathf.Clamp01(temperature);

        // show meters
        if (showAmount && amount > 0)
            DebugUtils.InGameMeter(amount / maxAmount, transform.position, 20, DrinkFoodUtils.GetFoodColor(foodType)); // TODO: food color?

        if (showTemperature && temperature > 0)
            DebugUtils.InGameMeter(temperature, transform.position, 30, Color.red);
    }

    public void DebugAmount(float offset)
    {
        DebugUtils.Meter(amount / maxAmount, transform.position, offset, DrinkFoodUtils.GetFoodColor(foodType));
    }
}
