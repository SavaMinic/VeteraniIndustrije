using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metla : Interactable
{
    private SpriteRenderer metlaRenderer;
    private float verticalInSlot;


    protected override void OnStart()
    {
        base.OnStart();
        metlaRenderer = GetComponentInChildren<SpriteRenderer>();
        verticalInSlot = metlaRenderer.transform.localPosition.y;
    }

    protected override void OnPlacedInSlot(Slot slot)
    {
        base.OnPlacedInSlot(slot);

        var pos = metlaRenderer.transform.localPosition;
        pos.y = verticalInSlot;
        metlaRenderer.transform.localPosition = pos;
    }

    protected override void OnRemovedFromSlot(Slot slot)
    {
        base.OnRemovedFromSlot(slot);

        var pos = metlaRenderer.transform.localPosition;
        pos.y = 0f;
        metlaRenderer.transform.localPosition = pos;
    }

    bool isSwiping;
    Vector3 posVelo;
    float angleVelo;
    float angle;

    Fleka fleka;

    public void Swipe(Interactable closestItem)
    {
        if (!closestItem) fleka = null;
        else if (closestItem is Fleka)
            fleka = closestItem as Fleka;
        else fleka = null;


        isSwiping = true;
        //Debug.Log("Swiping");
    }

    float progress;

    private void Update()
    {
        Vector3 targetPos = new Vector3(0.62f, 0, 0.68f); //Vector3.forward * 0.5f;//Vector3.forward;
        float targetAngle = 34.11f;

        if (isHeld)
        {
            if (isSwiping)
            {
                //targetPos =
                //Vector3.right * (0.4f * Mathf.Cos(Time.time * 20)) -
                //Vector3.forward * (0.4f * Mathf.Cos(Time.time * 20) - 1);

                progress += Time.deltaTime * 20 / Mathf.PI;
                if (progress > 1)
                {
                    Debug.Assert(Database.e != null, "Database.e is null");
                    Debug.Assert(Database.e.broomSweepClips != null, "Database.e.broomSweepClips is null");
                    Debug.Assert(Database.e.broomSweepClips.Length > 0, "No broomSweepClips");
                    Database.e.broomSweepClips.Play2D(true, 0.2f, pitch: 2);
                    progress = 0;

                    if (fleka)
                        fleka.Sweep();
                }


                targetAngle = 34.11f + Mathf.Sin(Time.time * 20) * 60;
            }

            isSwiping = false;

            angle = Mathf.SmoothDamp(angle, targetAngle, ref angleVelo, 0.05f);

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref posVelo, 0.05f);
            transform.localEulerAngles = new Vector3(angle, -147.2f, 0);
        }
    }
}
