using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodContainer : MonoBehaviour
{
    public Food foodType;
    public float amount;
    public float maxAmount = 1;
    public float temperature;

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

        if (foodContainer.amount > foodContainer.maxAmount) return false;

        foodContainer.amount += pourAmount;
        amount -= pourAmount;

        if (amount <= 0) amount = 0;

        foodContainer.temperature = temperature;

        return true;
    }

    private void Update()
    {
        temperature -= Time.deltaTime * coolingSpeed;
        temperature = Mathf.Clamp01(temperature);
    }

    public void DebugAmount(float offset)
    {
        DebugUtils.Meter(amount / maxAmount, transform.position, offset, Color.yellow);
    }
}
