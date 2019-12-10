using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Worst named thing ever
public interface IProximityFindable
{
    Vector3 ProximityPosition { get; }
    bool SkipProximitySearch { get; }
}
