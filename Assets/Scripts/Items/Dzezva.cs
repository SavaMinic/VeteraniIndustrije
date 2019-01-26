using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dzezva : Item
{
    public bool hasCoffee;
    public float water;

    public float temperature;
    public float kipi;

    public float heatingSpeed = 0.1f;
    public float kipiSpeed = 0.1f;
    public float coolingSpeed = 0.01f;

    public Ringla ringla;
    public Slavina slavina;

    private void Update()
    {
        if (ringla && ringla.isOn && water == 1)
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
            water += Time.deltaTime * slavina.fillSpeed;
        }

        temperature = Mathf.Clamp01(temperature);
        kipi = Mathf.Clamp01(kipi);
        water = Mathf.Clamp01(water);

        // Debug lines
        DebugLine(temperature, 0.2f, Color.red);
        DebugLine(kipi, 0.3f, Color.yellow);
        DebugLine(water, 0.4f, Color.blue);
    }

    void DebugLine(float value, float dist, Color color)
    {
        Debug.DrawRay(transform.position + Vector3.right * dist, Vector3.up, Color.gray);
        Debug.DrawRay(transform.position + Vector3.right * dist, Vector3.up * value, color);
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
