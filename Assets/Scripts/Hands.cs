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

    List<Slot> slotsToSearch = new List<Slot>();
    List<Interactable> interactablesToSearch = new List<Interactable>();

    private void Update()
    {
        if (!Application.isPlaying || GameController.I.IsPaused)
            return;

        Vector3 myPos = transform.position;

        slotsToSearch.Clear();

        if (heldItem)
        {
            for (int i = 0; i < Slot.all.Length; i++)
            {
                Slot slot = Slot.all[i];

                // skip if slot is occupied
                if (slot.itemInSlot) continue;
                // skip if slot only interacts with an item and this item is not held
                if (slot.onlyInteractsWith && slot.onlyInteractsWith != heldItem) continue;
                // skip if slot uses interaction control and held item does not correspond
                if (slot.useInteractionControl && !InteractionControl.I.CanInteract(heldItem.gameObject, slot.gameObject)) continue;
                // special metla slot
                if (heldItem is Metla && slot.name != "MetlaSlot") continue;

                slotsToSearch.Add(Slot.all[i]);
            }
        }

        Slot closestSlot = Util.FindClosest(slotsToSearch, myPos, interactRange, useViewSpaceDistance);


        interactablesToSearch.Clear();
        for (int i = 0; i < Interactable.all.Count; i++)
        {
            Interactable interactable = Interactable.all[i];
            // skip held item
            if (interactable == heldItem) continue;
            // don't interact with other player's item
            if (interactable.isHeld) continue;
            // prevent coffee interacting with dzezva if dzezva is empty
            if (interactable is Dzezva && heldItem is Coffee && interactable.GetComponent<Container>().amount == 0) continue;
            // don't interact with guests if nothing in hands
            if (!heldItem && interactable is Consumer) continue;
            // skip flekas if nothing is held or not a metla is held
            if (interactable is Fleka && (!heldItem || (heldItem && !(heldItem is Metla)))) continue;
            // with metla interact only with doors and flekas
            if (heldItem && heldItem is Metla && !(interactable is Fleka || interactable is Door)) continue;
            interactablesToSearch.Add(interactable);
        }

        Interactable closestItem = Util.FindClosest(interactablesToSearch, myPos, interactRange, useViewSpaceDistance);

        if (closestItem)
            Debug.Log(closestItem.name);

        // Give priority to slot or item depending what's closer
        if (closestSlot && closestItem)
        {
            if (closestItem.IsCloserThan((IProximityFindable)closestSlot, myPos, true))
                closestSlot = null;
            else
                closestItem = null;
        }

        if (closestItem)
        {
            Debug.DrawLine(myPos, closestItem.ProximityPosition, Color.green);

            closestItem.Highlight(true);
        }
        else if (closestSlot)
        {
            Debug.DrawLine(transform.position, closestSlot.ProximityPosition, Color.red);

            closestSlot.Highlight(true);
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