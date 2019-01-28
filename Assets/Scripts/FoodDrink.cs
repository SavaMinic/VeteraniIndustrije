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
        if (!Application.isPlaying) return Color.clear;

        return Database.e.drinkColors[(int)drink];

        /*
        switch (drink)
        {
            case Drink.Coffee: return ColorDB.e.drinkColors[0];
            case Drink.Rakija: return Color.white;
            case Drink.Water: return Color.blue;
            case Drink.CrniSok: return Color.black;
            case Drink.ZutiSok: return Color.yellow;
            default: return Color.white;
        }*/
    }

    public static Color GetFoodColor(Food food)
    {
        switch (food)
        {
            case Food.Sarma: return Color.yellow;
            case Food.Salata: return Color.green;
            case Food.Orasnica: return Color.gray;
            default: return Color.white;
        }
    }
}
