using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zavese : MonoBehaviour
{
    public GameObject openableObject;

    Animator _animator;
    Animator animator { get { if (!_animator) _animator = GetComponent<Animator>(); return _animator; } }

    IOpenable openable;

    private void Start()
    {
        openable = openableObject.GetComponent<IOpenable>();
    }

    private void Update()
    {
        if (Promaja.IsActive && openable.IsOpen)
            animator.SetBool("Promaja", true);
        else
            animator.SetBool("Promaja", false);
    }
}
