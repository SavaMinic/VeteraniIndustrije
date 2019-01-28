using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database e;
    private void Awake() { e = this; }

    public Consumable[] foods;
    public Consumable[] drinks;

    public Color[] drinkColors;
}
