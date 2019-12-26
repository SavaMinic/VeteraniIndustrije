using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public int player = 1;
    public Interactable heldItem;

    public Vector3 holdOffset;
    public float interactRange = 1;
    public bool useViewSpaceDistance = true;

    Interactable lastClosestItem;
    Slot lastClosestSlot;

    public PouringSound pouringSound;

    private void Update()
    {
        if (!Application.isPlaying || GameController.I.IsPaused)
            return;

        Slot closestSlot = Util.FindClosest(Slot.all, null, transform.position, interactRange, useViewSpaceDistance,
            s => s.itemInSlot);
        if (closestSlot)
        {
            Debug.DrawLine(transform.position, closestSlot.ProximityPosition, Color.red);
            // if we hold item...
            if (heldItem && (!closestSlot.onlyInteractsWith || closestSlot.onlyInteractsWith == heldItem)
                // ... and slot is either generic ...
                && (!closestSlot.useInteractionControl ||
                 // or it can interact with the item
                 InteractionControl.I.CanInteract(heldItem.gameObject, closestSlot.gameObject)))
                closestSlot.Highlight(true);
        }

        Interactable closestItem = Util.FindClosest(Interactable.all, heldItem, transform.position, interactRange, useViewSpaceDistance);
        if (closestItem)
        {
            Debug.DrawLine(transform.position, closestItem.ProximityPosition, Color.green);
            // Door is always highlighted
            if (closestItem is Door)
            {
                closestItem.Highlight(true);
            }
            // if we are not holding or if we are holding and this item is interactable
            if (!heldItem || InteractionControl.I.CanInteract(heldItem.gameObject, closestItem.gameObject))
            {
                if (!(closestItem is Fleka) &&
                    !(heldItem is Dzezva && closestItem is Coffee) &&
                    !(heldItem is Coffee && closestItem is Dzezva && (closestItem as Dzezva).container.amount <= 0))
                {
                    closestItem.Highlight(true);
                }
            }
            /*
            // FLEKA IS SPECIAL
            if (closestItem is Fleka)
            {
                closestItem.Highlight(heldItem is Metla);
            }*/
        }

        if (closestItem != lastClosestItem && lastClosestItem)
            lastClosestItem.Highlight(false);

        if (closestSlot != lastClosestSlot && lastClosestSlot)
            lastClosestSlot.Highlight(false);

        lastClosestItem = closestItem;
        lastClosestSlot = closestSlot;

        DomacinInputManager.Actions p = player == 1 ?
            DomacinInputManager.e.p1 :
            DomacinInputManager.e.p2;

        bool btn_down = p.interactAction.triggered;
        bool btn_held = p.interactAction.ReadValue<float>() == 1;

        if (closestItem is Gajba && heldItem is Pivo && btn_down)
        {
            Pivo pivo = heldItem as Pivo;
            pivo.Remove();
            (closestItem as Gajba).putClip.Play2D(0.5f);
            Debug.Log("Pivo removed!");
        }
        else if (closestItem is Gajba && !heldItem && btn_down)
        {
            Interactable pivo = Database.e.CreatePivo();
            (closestItem as Gajba).pickupClip.Play2D(0.5f);
            heldItem = pivo.Take(this);
        }
        // Sibice
        else if (heldItem is Matches && closestItem is Candle candle && btn_down)
        {
            if (!candle.isBurning)
                candle.Ignite();
        }
        // Davanje flase piva
        else if (closestItem is Consumer && heldItem is Pivo && btn_down)
        {
            var gost = closestItem as Consumer;
            Consumable pivoType = null;
            foreach (var consumable in Database.e.consumables)
                if (consumable.wishType == GuestWish.GuestWishType.Pivo)
                    pivoType = consumable;
            gost.drinkContainer.AddDrink(pivoType, 1);
            Pivo pivo = heldItem as Pivo;
            pivo.Remove();
        }
        // Sipanje hrane i pica u gosta
        else if (closestItem is Consumer && heldItem?.GetComponent<Container>())
        {
            Consumer gost = closestItem as Consumer;
            Container drc = heldItem.GetComponent<Container>();

            if (drc.containsType == Container.ContainsType.Food && btn_down)
                drc.TransferTo(gost.foodContainer, 1);

            if (drc.containsType == Container.ContainsType.Drink && btn_held)
            {
                pouringSound?.Pour();
                drc.TransferTo(gost.drinkContainer); // TODO: Tweak speed
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
        else if (closestItem is Door && btn_down)
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
        else if (heldItem?.GetComponent<Container>() && closestItem?.GetComponent<Container>()
                 // if this 2 items can interact
                 && InteractionControl.I.CanInteract(heldItem?.gameObject, closestItem?.gameObject))
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
                if (closestItem is Fleka && heldItem is Metla)
                {

                }
                // Special case with metla slot to prevent placing anywhere else
                else if (closestSlot && heldItem is Metla && closestSlot.name == "MetlaSlot")
                {
                    if (heldItem.PlaceInSlot(closestSlot))
                    {
                        closestSlot.Highlight(false);
                        heldItem = null;
                    }
                }
                else if (closestSlot && !(heldItem is Metla))
                {
                    if (heldItem.PlaceInSlot(closestSlot))
                    {
                        closestSlot.Highlight(false);
                        heldItem = null;
                    }
                }
            }
        }

        if (btn_held && heldItem is Metla)
        {
            var metla = heldItem as Metla;
            metla.Swipe(closestItem);
        }
    }
}