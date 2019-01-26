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

        // Sipanje kafe u solju:
        if (closestItem is Cup && heldItem is Dzezva)
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
        if (closestItem is Cup && heldItem is Bottle) // copied
        {
            if (Input.GetKey(interactionKey))
            {
                var bottle = heldItem as Bottle;
                var cup = closestItem as Cup;

                Debug.DrawLine(transform.position, cup.transform.position);

                if ((cup.amount == 0 || cup.drinkType == bottle.drinkType) && bottle.amount > 0)
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
        // Take / Place item
        else if (Input.GetKeyDown(interactionKey))
        {
            if (!heldItem)
            {
                if (closestItem)
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