using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumer : Interactable
{
    public Container foodContainer;
    public Container drinkContainer;

    public float drinkingSpeed = 0.1f;
    public float eatingSpeed = 0.1f;

    private void Update()
    {
        foodContainer.DebugAmount(0.2f);
        drinkContainer.DebugAmount(0.4f);

        // Gost jede
        if (foodContainer.amount > 0)
        {
            var speed = foodContainer.type.wishType == GuestWish.GuestWishType.Zito ? 0.95f : eatingSpeed;
            foodContainer.amount -= Time.deltaTime * speed;
        }

        // Gost pije
        if (drinkContainer.amount > 0)
        {
            drinkContainer.amount -= Time.deltaTime * drinkingSpeed;
        }
    }
}
