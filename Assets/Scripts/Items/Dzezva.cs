using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dzezva : Interactable
{
    public Container container;
    public bool hasCoffee;

    public float heatingSpeed = 0.1f;

    public Ringla ringla;

    public GameObject iskipBrbotj;

    public void ShowIskipBrbotj(bool enable)
    {
        iskipBrbotj.SetActive(enable);
    }

    private void Update()
    {
        if (!Application.isPlaying || GameController.I.IsPaused)
            return;

        if (ringla && ringla.isOn)
            container.AddHeat(Time.deltaTime * heatingSpeed);

        if (container.type.name == "Kafa" &&
            container.amount > 0.7f &&
            container.temperature >= 0.98f)
        {
            ShowIskipBrbotj(true);
        }
        else ShowIskipBrbotj(false);

        // Debug lines
        DebugUtils.Meter(container.temperature, transform.position, 0.2f, Color.red);
        DebugUtils.Meter(container.amount, transform.position, 0.4f, container.type.color);
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
