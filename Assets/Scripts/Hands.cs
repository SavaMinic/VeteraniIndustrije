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

        // Special cases:

        // Sipanje sarme u gosta
        if (closestItem is Consumer && heldItem && heldItem.GetComponent<FoodContainer>())
        {
            if (btn_down)
            {
                Consumer gost = closestItem as Consumer;
                Container drc = heldItem.GetComponent<Container>();

                bool success = drc.TransferTo(gost.foodContainer, 1);
            }
        }
        // Sipanje pica u gosta
        else if (closestItem is Consumer && heldItem && heldItem.GetComponent<DrinkContainer>())
        {
            if (btn_held)
            {
                Consumer gost = closestItem as Consumer;
                Container drc = heldItem.GetComponent<Container>();

                bool success = drc.TransferTo(gost.drinkContainer, Time.deltaTime); // TODO: Tweak speed

                //if (success) Debug.Log("Sipa se u gosta");
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

        // Sipanje sarme iz lonca u tanjir
        else if (closestItem is Plate && heldItem is LonacSarme)
        {
            if (Input.GetAxis(interactionKey) > 0f)
            {
                var plateContainer = closestItem.GetComponent<FoodContainer>();
                var lonacContainer = heldItem.GetComponent<FoodContainer>();

                bool success = lonacContainer.TransferFoodTo(plateContainer, 1);

                if (success) { } // play sound, effects

                /*
                var plate = closestItem as Plate;
                var lonac = heldItem as LonacSarme;

                if (plate.foodContainer.amount == 0)
                {
                    plate.foodContainer.foodType = Food.Sarma;
                    plate.foodContainer.amount = 1;
                    // intantiate sarma
                }*/
            }
        }
        // Sipanje kafe u solju:
        else if (heldItem is Dzezva && closestItem is Cup)
        {
            if (btn_held)
            {
                var dzezva = heldItem as Dzezva;
                var cup = closestItem as Cup;

                Debug.DrawLine(transform.position, cup.transform.position);

                dzezva.container.TransferTo(cup.container, Time.deltaTime); // TODO: Add speed

                /*
                if ((cup.amount == 0 || cup.drinkType == dzezva.drinkType) && dzezva.amount > 0)
                {
                    cup.drinkType = dzezva.drinkType;

                    float pourAmount = Time.deltaTime * 1; // TODO: add mult

                    dzezva.amount -= pourAmount;
                    if (dzezva.amount < 0) pourAmount += Mathf.Abs(dzezva.amount);

                    cup.amount += pourAmount;
                }*/
            }
        }
        // Sipanje iz flase u solju/casu - kopirano ozgo
        else if (closestItem is Cup && heldItem is Bottle)
        {
            if (btn_held)
            {
                var bottle = heldItem as Bottle;
                var cup = closestItem as Cup;

                Debug.DrawLine(transform.position, cup.transform.position);

                bottle.container.TransferTo(cup.container, Time.deltaTime); // TODO: Add speed

                /*
                if ((cup.amount == 0 || cup.drinkType == bottle.drinkType)
                    && bottle.amount > 0 && cup.amount < 1)
                {
                    cup.drinkType = bottle.drinkType;

                    float pourAmount = Time.deltaTime * 1; // TODO: add mult

                    bottle.amount -= pourAmount;
                    if (bottle.amount < 0) pourAmount += Mathf.Abs(bottle.amount);

                    cup.amount += pourAmount;
                }*/
            }
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
        // TODO: metla
        // General case: Take / Place item
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