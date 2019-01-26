using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public Item heldItem;

    public Vector3 holdOffset;

    private void Update()
    {
        KeyCode interactionKey = KeyCode.Space;

        Slot closestSlot = Slot.FindClosest(transform.position);
        Debug.DrawLine(transform.position, closestSlot.transform.position, Color.red);

        Item closestItem = Item.FindClosest(transform.position);
        Debug.DrawLine(transform.position, closestItem.transform.position, Color.green);

        // Special cases:

        // Sipanje kafe u solju:
        if (closestItem is Cup && heldItem is Dzezva)
        {
            if (Input.GetKey(interactionKey))
            {
                Dzezva dzezva = heldItem as Dzezva;
                Cup cup = closestItem as Cup;

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
        // Sipanje kafe u dzezvu:
        else if (closestItem is Dzezva && heldItem is Coffee)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                Dzezva dzezva = closestItem as Dzezva;
                dzezva.drinkType = Drink.Coffee;
            }
        }
        // TODO: metla
        // Take / Place item
        else if (Input.GetKeyDown(interactionKey))
        {
            if (!heldItem)
            {
                Item item = closestItem.Take(this);

                if (item != null)
                    heldItem = item;
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