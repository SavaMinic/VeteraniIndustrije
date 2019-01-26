using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public Item heldItem;

    public Vector3 holdOffset;
    public float interactRange = 1;

    private void Update()
    {
        KeyCode interactionKey = KeyCode.Space;

        Slot closestSlot = Slot.FindClosest(transform.position, interactRange);
        if (closestSlot) Debug.DrawLine(transform.position, closestSlot.transform.position, Color.red);

        Item closestItem = Item.FindClosest(transform.position, interactRange);
        if (closestItem) Debug.DrawLine(transform.position, closestItem.transform.position, Color.green);

        // Special cases:

        // Menjanje kanala TV
        if (!heldItem && closestItem is TV)
        {
            var tv = closestItem as TV;

            tv.SwitchChannel();
        }
        // Otvaranje vrata i prozora:
        if (!heldItem && closestItem is Door)
        {
            var door = closestItem as Door;

            door.Toggle();
        }
        // Sipanje sarme iz lonca u tanjir
        if (closestItem is Plate && heldItem is LonacSarme)
        {
            if (Input.GetKeyDown(interactionKey))
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
        else if (closestItem is Cup && heldItem is Dzezva)
        {
            if (Input.GetKey(interactionKey))
            {
                var dzezva = heldItem as Dzezva;
                var cup = closestItem as Cup;

                Debug.DrawLine(transform.position, cup.transform.position);

                if ((cup.amount == 0 || cup.drinkType == dzezva.drinkType) && dzezva.amount > 0)
                {
                    cup.drinkType = dzezva.drinkType;

                    float pourAmount = Time.deltaTime * 1; // TODO: add mult

                    dzezva.amount -= pourAmount;
                    if (dzezva.amount < 0) pourAmount += Mathf.Abs(dzezva.amount);

                    cup.amount += pourAmount;
                }
            }
        }
        // Sipanje iz flase u solju/casu - kopirano ozgo
        if (closestItem is Cup && heldItem is Bottle)
        {
            if (Input.GetKey(interactionKey))
            {
                var bottle = heldItem as Bottle;
                var cup = closestItem as Cup;

                Debug.DrawLine(transform.position, cup.transform.position);

                if ((cup.amount == 0 || cup.drinkType == bottle.drinkType)
                    && bottle.amount > 0 && cup.amount < 1)
                {
                    cup.drinkType = bottle.drinkType;

                    float pourAmount = Time.deltaTime * 1; // TODO: add mult

                    bottle.amount -= pourAmount;
                    if (bottle.amount < 0) pourAmount += Mathf.Abs(bottle.amount);

                    cup.amount += pourAmount;
                }
            }
        }
        // Sipanje kafe u dzezvu:
        else if (closestItem is Dzezva && heldItem is Coffee)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                Dzezva dzezva = closestItem as Dzezva;
                dzezva.drinkType = Drink.Coffee;
                Debug.Log("Dzezva");
            }
        }
        // TODO: metla
        // General case: Take / Place item
        else if (Input.GetKeyDown(interactionKey))
        {
            if (!heldItem)
            {
                if (closestItem && closestItem.isTakeable)
                {
                    Item item = closestItem.Take(this);

                    if (item != null)
                        heldItem = item;
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