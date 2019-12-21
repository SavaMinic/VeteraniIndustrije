using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateInputManagerIfNoneExists : MonoBehaviour
{
    public GameObject inputManager;

    void Start()
    {
        if (FindObjectsOfType<DomacinInputManager>().Length == 0)
            inputManager.SetActive(true);
    }
}
