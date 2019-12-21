using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Termometar : MonoBehaviour
{
    public SpriteRenderer sprite;
    public SpriteMask mask;

    public float scaleMult;

    public Container container;
    public Vector3 offset = Vector3.one;

    private void Update()
    {
        if (container.temperature <= 0)
            transform.position = Vector3.one * 1000;
        else
            transform.position = container.transform.position + offset;

        transform.rotation = container.transform.rotation;

        Vector3 s = mask.transform.localScale;
        s.y = container.temperature * scaleMult;
        mask.transform.localScale = s;

        //sprite.color = container.type.color;
    }
}
