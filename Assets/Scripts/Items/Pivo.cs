using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivo : Interactable
{
    InteractionControl.InteractionData id;

    protected override void OnStart()
    {
        var gajba = FindObjectOfType<Gajba>().gameObject;

        if (!gajba) return;

        id = new InteractionControl.InteractionData();
        id.interactionObject1 = gameObject;
        id.interactionObject2 = gajba;

        InteractionControl.I.InteractionObjectData.Add(id);
    }

    private void OnDestroy()
    {
        InteractionControl.I.InteractionObjectData.Remove(id);
    }
}
