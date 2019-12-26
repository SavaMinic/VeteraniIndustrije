using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ContainerMeter : MonoBehaviour
{
    public RectTransform rectTransform;

    public Container container;
    public Image maskingImage;

    public Vector3 offset;

    Canvas canvas;

    private void Start()
    {
        canvas = CanvasController.I.GetComponent<Canvas>();
    }

    private void Update()
    {
        if (container == null)
        {
            Destroy(gameObject);
            return;
        }

        if (container.amount <= 0)
            rectTransform.anchoredPosition = Vector3.one * 10000;
        else
        {
            Vector3 pos = Util.WorldToUISpace(canvas, container.transform.position);
            //Vector3 pos = Camera.main.WorldToScreenPoint(container.transform.position);
            rectTransform.position = pos + offset;

            maskingImage.color = container.type.color;
            maskingImage.fillAmount = container.amount;
        }
    }
}
