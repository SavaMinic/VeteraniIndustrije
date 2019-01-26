using UnityEngine;

public enum Drink
{
    Coffee,
    Rakija,
    Water,
    CrniSok,
    ZutiSok
}

public enum Food
{
    Sarma,
    Salata,
    Orasnica
}

public static class DrinkFoodUtils
{
    public static Color GetColor(Drink drink)
    {
        switch (drink)
        {
            case Drink.Coffee: return Color.black;
            case Drink.Rakija: return Color.white;
            case Drink.Water: return Color.blue;
            case Drink.CrniSok: return Color.black;
            case Drink.ZutiSok: return Color.yellow;
            default: return Color.white;
        }
    }
}
