using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleka : Interactable
{

    public static int Count;
    public static bool Prljavo => Count >= 3;

    public static float GuestWishModifier => Prljavo ? 1.4f : 1f;
    
    private SpriteRenderer spriteRenderer;
    private bool isFading;
    
    protected override void OnStart()
    {
        base.OnStart();

        spriteRenderer = GetComponent<SpriteRenderer>();
        
        Count++;
    }

    public void Remove()
    {
        if (isFading)
            return;

        isFading = true;
        Count--;
        StartCoroutine(DelayDestroy(0.8f));
    }
    
    private IEnumerator DelayDestroy(float delay)
    {
        var startColor = Color.white;
        var endColor = new Color(1f, 1f, 1f, 0f);
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / delay)
        {
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        Destroy(gameObject);
    }
}
