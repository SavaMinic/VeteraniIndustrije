using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motion : MonoBehaviour
{
    RectTransform _rt;
    RectTransform rt { get { if (!_rt) _rt = GetComponent<RectTransform>(); return _rt; } }

    public AnimationCurve curve;

    void Update()
    {
        rt.anchoredPosition = new Vector2(0, curve.Evaluate(Time.time));
    }
}
