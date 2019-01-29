using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public string interactionKey = "Fire1";
    public Interactable heldItem;

    public Vector3 holdOffset;
    public float interactRange = 1;
    public bool useViewSpaceDistance = true;

    Interactable lastClosestItem;
    Slot lastClosestSlot;

    public PouringSound pouringSound;

    private void Update()
    {
        Slot closestSlot = Util.FindClosest(Slot.all, null, transform.position, interactRange, useViewSpaceDistance);
        if (closestSlot)
        {
            Debug.DrawLine(transform.position, closestSlot.transform.position, Color.red);
            if (heldItem)
                closestSlot.Highlight(true);
        }

        Interactable closestItem = Util.FindClosest(Interactable.all, heldItem, transform.position, interactRange, useViewSpaceDistance);
        if (closestItem)
        {
            Debug.DrawLine(transform.position, closestItem.transform.position, Color.green);
            closestItem.Highlight(true);
        }

        if (closestItem != lastClosestItem && lastClosestItem)
            lastClosestItem.Highlight(false);

        if (closestSlot != lastClosestSlot && lastClosestSlot)
            lastClosestSlot.Highlight(false);

        lastClosestItem = closestItem;
        lastClosestSlot = closestSlot;

        bool btn_down = Input.GetButtonDown(interactionKey);
        bool btn_held = Input.GetAxis(interactionKey) > 0f;

        // Sipanje hrane i pica u gosta
        if (closestItem is Consumer && heldItem?.GetComponent<Container>())
        {
            Consumer gost = closestItem as Consumer;
            Container drc = heldItem.GetComponent<Container>();

            if (drc.containsType == Container.ContainsType.Food && btn_down)
                drc.TransferTo(gost.foodContainer, 1);

            if (drc.containsType == Container.ContainsType.Drink && btn_held)
            {
                pouringSound?.Pour();
                drc.TransferTo(gost.drinkContainer, Time.deltaTime); // TODO: Tweak speed
            }
        }
        // Chop and take salata
        else if (!heldItem && closestItem is Salata && btn_down)
        {
            var salata = closestItem as Salata;

            if (!salata.IsChoppedAndReady)
                salata.Chop();
            else
                salata.Take(this);
        }
        // Menjanje kanala TV
        else if (!heldItem && closestItem is TV && btn_down)
        {
            var tv = closestItem as TV;

            tv.SwitchChannel();
        }
        // Otvaranje vrata i prozora:
        else if (!heldItem && closestItem is Door && btn_down)
        {
            var door = closestItem as Door;
            door.Toggle();
        }
        if (!heldItem && closestItem is Prozor && btn_down)
        {
            var prozor = closestItem as Prozor;
            prozor.Toggle();
        }
        // Sipanje kafe u dzezvu:
        else if (closestItem is Dzezva && heldItem is Coffee)
        {
            if (btn_down)
            {
                Dzezva dzezva = closestItem as Dzezva;
                dzezva.container.type = (heldItem as Coffee).coffeeConsumable;
            }
        }
        // General pouring stuff from container to container
        else if (heldItem?.GetComponent<Container>() && closestItem?.GetComponent<Container>())
        {
            Container inC = heldItem.GetComponent<Container>();
            Container toC = closestItem.GetComponent<Container>();

            if (inC.containsType == Container.ContainsType.Drink &&
                toC.containsType == Container.ContainsType.Drink &&
                btn_held)
                inC.TransferTo(toC, Time.deltaTime);
        }
        // Take / Place item
        else if (btn_down)
        {
            if (!heldItem)
            {
                if (closestItem && closestItem.isTakeable)
                {
                    Interactable item = closestItem.Take(this);

                    if (item != null)
                    {
                        heldItem = item;
                        item.Highlight(false);
                    }
                }
            }
            else
            {
                if (closestSlot)
                {
                    heldItem.PlaceInSlot(closestSlot);
                    heldItem = null;
                }
            }
        }
    }
}