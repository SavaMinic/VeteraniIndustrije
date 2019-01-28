﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dzezva : Interactable
{
    public DrinkContainer container;
    public bool hasCoffee;

    public float heatingSpeed = 0.1f;

    public Ringla ringla;

    private void Update()
    {
        if (ringla && ringla.isOn && container.amount >= 1)
            container.AddHeat(Time.deltaTime * heatingSpeed);

        // Debug lines
        DebugUtils.Meter(container.temperature, transform.position, 0.2f, Color.red);
        DebugUtils.Meter(container.amount, transform.position, 0.4f, DrinkFoodUtils.GetColor(container.drinkType));
    }

    protected override void OnPlacedInSlot(Slot slot)
    {
        if (slot is Ringla)
            ringla = slot as Ringla;
    }

    protected override void OnRemovedFromSlot(Slot slot)
    {
        ringla = null;
    }
}
