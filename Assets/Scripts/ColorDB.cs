using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDB : MonoBehaviour
{
    public static ColorDB e;
    private void Awake()
    {
        e = this;
    }

    public Color[] drinkColors;
}
