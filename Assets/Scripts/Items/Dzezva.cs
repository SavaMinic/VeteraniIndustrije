using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dzezva : Item
{
    public bool hasCoffee;
    public float amount;

    public Drink drinkType = Drink.Water;

    public float temperature;
    public float kipi;

    public float heatingSpeed = 0.1f;
    public float kipiSpeed = 0.1f;
    public float coolingSpeed = 0.01f;

    public Ringla ringla;
    public Slavina slavina;

    private void Update()
    {
        if (ringla && ringla.isOn && amount == 1)
        {
            temperature += Time.deltaTime * heatingSpeed;

            if (temperature >= 1)
                kipi += Time.deltaTime * kipiSpeed;
        }
        else
        {
            temperature -= Time.deltaTime * coolingSpeed;
            kipi -= Time.deltaTime * kipiSpeed;
        }

        if (slavina)
        {
            amount += Time.deltaTime * slavina.fillSpeed;
        }

        temperature = Mathf.Clamp01(temperature);
        kipi = Mathf.Clamp01(kipi);
        amount = Mathf.Clamp01(amount);

        // Debug lines
        DebugUtils.Meter(temperature, transform.position, 0.2f, Color.red);
        DebugUtils.Meter(kipi, transform.position, 0.3f, Color.yellow);
        DebugUtils.Meter(amount, transform.position, 0.4f, Color.blue);
    }

    protected override void OnPlacedInSlot(Slot slot)
    {
        if (slot is Ringla)
            ringla = slot as Ringla;

        if (slot is Slavina)
        {
            slavina = slot as Slavina;
            slavina.PourWater();
        }
    }

    protected override void OnRemovedFromSlot(Slot slot)
    {
        if (slavina)
        {
            slavina.EndPouringWater();
        }

        ringla = null;
        slavina = null;
    }
}
