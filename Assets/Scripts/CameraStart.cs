using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class CameraStart : MonoBehaviour
{
    public Transform startPos;
    public Transform targetPos;

    private void Awake()
    {
        transform.position = startPos.position;
    }

    void Start()
    {
        transform.DOMove(targetPos.position, 0.8f).
            SetEase(Ease.OutCubic);
    }

    void Update()
    {

    }
}
