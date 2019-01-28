using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public enum ContainsType { Food, Drink }
    public ContainsType containsType;

    public Consumable type;

    public float amount;
    public float maxAmount = 1;
    public float temperature;
    public float coolingSpeed = 0.01f;

    public bool canBeRefilledAtSlavina;

    public bool showAmount = true;
    public bool showTemperature = true;

    const float boilingLossSpeed = 0.05f;

    public bool TransferTo(Container toContainer, float pourAmount)
    {
        if (amount <= 0) return false;

        if (pourAmount <= 0) return false;

        if (toContainer.amount > 0 && type != toContainer.type)
            return false;

        if (toContainer.amount > toContainer.maxAmount)
        {
            toContainer.amount = toContainer.maxAmount;
            return false;
        }

        toContainer.type = type;

        toContainer.amount += pourAmount;
        amount -= pourAmount;

        if (amount <= 0) amount = 0;

        toContainer.temperature = temperature;

        return true;
    }

    #region Public

    public void AddDrink(Consumable _type, float _amount)
    {
        type = _type;
        amount += _amount;
        amount = Mathf.Clamp(amount, 0, maxAmount);
    }

    public void AddHeat(float heat)
    {
        temperature += heat;
        temperature = Mathf.Clamp01(temperature);
    }

    public bool IsAtGoodTemperature()
    {
        Debug.Assert(type, "Type is null. Not allowed!");

        if (type.goodTemperatureSide == Consumable.Side.Below
        && temperature <= type.goodTemperatureLevel)
            return true;

        if (type.goodTemperatureSide == Consumable.Side.Above
            && temperature >= type.goodTemperatureLevel)
            return true;

        return false;
    }

    #endregion

    private void Update()
    {

        if (temperature >= 1)
            amount -= Time.deltaTime * boilingLossSpeed;

        // constant cooling
        temperature -= Time.deltaTime * coolingSpeed;

        // clamp
        amount = Mathf.Clamp(amount, 0, maxAmount);
        temperature = Mathf.Clamp01(temperature);

        // show meters
        if (showAmount && amount > 0)
            DebugUtils.InGameMeter(amount / maxAmount, transform.position, 20, type.color);

        if (showTemperature && temperature > 0)
            DebugUtils.InGameMeter(temperature, transform.position, 30, Color.red);
    }

    public void DebugAmount(float offset)
    {
        DebugUtils.Meter(amount / maxAmount, transform.position, offset, type.color);
    }
}