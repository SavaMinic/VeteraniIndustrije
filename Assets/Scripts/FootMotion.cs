using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootMotion : MonoBehaviour
{
    Rigidbody _rb;
    Rigidbody rb { get { if (!_rb) _rb = GetComponent<Rigidbody>(); return _rb; } }

    public Transform legLeft;
    public Transform legRight;

    public float sidewaysMult = 0.2f;
    public float heightMult = 0.3f;

    class Leg
    {
        public Transform t;
        public float offset;
        public Vector3 startPos;
        public float x;
        public float y;
    }

    Leg left;
    Leg right;

    private void Start()
    {
        left = new Leg()
        {
            t = legLeft,
            offset = 1,
            startPos = legLeft.localPosition,
        };

        right = new Leg()
        {
            t = legRight,
            offset = 1,
            startPos = legRight.localPosition,
        };
    }

    Vector3 velo;

    void Update()
    {
        velo = Camera.main.transform.InverseTransformDirection(rb.velocity);
        velo *= 0.001f;

        UpdateLeg(left, 0);
        UpdateLeg(right, 0.5f);
    }

    void UpdateLeg(Leg leg, float offset)
    {
        leg.x += velo.x * Time.deltaTime * 60;
        float xt = -(leg.x + offset) * 30;

        float xstat = Mathf.Clamp01(Mathf.Abs(velo.x) * 100);
        float ystat = Mathf.Clamp01(velo.magnitude * 100);

        leg.y += Mathf.Max(Mathf.Abs(velo.y), Mathf.Abs(velo.x)) * Time.deltaTime * 60;
        float yt = (leg.y + offset) * 30;

        float x = Mathf.Cos(xt) * sidewaysMult * xstat;
        float y = Mathf.Sin(yt) * heightMult * ystat;
        y = Mathf.Clamp(y, 0, 1);

        Vector3 addPos = new Vector3(x, y, 0);
        leg.t.localPosition = leg.startPos + addPos;
    }
}
