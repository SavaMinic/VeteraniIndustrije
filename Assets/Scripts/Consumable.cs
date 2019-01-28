using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Prokuvo/Consumable")]
public class Consumable : ScriptableObject
{
    public Color color = Color.white;

    public float goodTemperatureLevel = 0.5f;
    public enum Side { Below, Above }
    public Side goodTemperatureSide;

    [System.Serializable]
    public class Quips
    {
        public string[] perfect;
        public string[] wrongTemperature;
        public string[] wrongConsumable;
        public string[] tooLate;
    }

    public Quips quips;
}
