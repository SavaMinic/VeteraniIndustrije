using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zavese : MonoBehaviour
{
    Animator _animator;
    Animator animator { get { if (!_animator) _animator = GetComponent<Animator>(); return _animator; } }

    private void Update()
    {
        if (Promaja.IsActive)
            animator.SetBool("Promaja", true);
        else
            animator.SetBool("Promaja", false);
    }
}
