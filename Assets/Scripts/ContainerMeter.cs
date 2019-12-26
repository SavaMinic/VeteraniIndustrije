using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ContainerMeter : MonoBehaviour
{
    public RectTransform rectTransform;

    public Container container;
    public Image maskingImage;

    private void Update()
    {
        if (container == null)
        {
            Destroy(gameObject);
            return;
        }

        if (container.temperature <= 0)
            rectTransform.anchoredPosition = Vector3.one * 10000;
        else
        {
            rectTransform.anchoredPosition = container.transform.position; // offset

            maskingImage.color = container.type.color;
            maskingImage.fillAmount = container.amount;
        }
    }
}
