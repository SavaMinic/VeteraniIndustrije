using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell : MonoBehaviour
{
    public static Bell e;
    void Awake() { e = this; }

    [SerializeField] AudioSource ring;

    public float ringingTime;

    bool isRinging;

    [SerializeField] GameObject ringingSprite;
    [SerializeField] GameObject staticSprite;

    public void Ring()
    {
        if (isRinging) return;

        isRinging = true;

        ringingSprite.SetActive(true);
        staticSprite.SetActive(false);

        ring.Play();
        StartCoroutine(EndRing());
    }

    IEnumerator EndRing()
    {
        yield return new WaitForSeconds(2);

        ringingSprite.SetActive(false);
        staticSprite.SetActive(true);

        isRinging = false;
    }

}
