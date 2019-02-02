using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerSpriteMask : MonoBehaviour
{
    public SpriteRenderer sprite;
    public SpriteMask mask;

    Container _container;
    Container container { get { if (!_container) _container = GetComponent<Container>(); return _container; } }

    public float scaleMult;

    private void Update()
    {
        Vector3 s = mask.transform.localScale;
        s.y = (container.amount / container.maxAmount) * scaleMult;
        mask.transform.localScale = s;

        sprite.color = container.type.color;
    }
}
