using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuestWishPanel : MonoBehaviour
{
    #region Properties/Fields

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private float fadeDuration;

    [SerializeField]
    private Image wishBackground;

    [SerializeField]
    private Image wishImage;

    private CanvasGroup canvasGroup;

    private Guest guest;

    private bool isShown;

    #endregion

    #region Mono

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        isShown = false;
        canvasGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (!Application.isPlaying || guest == null)
            return;

        if (guest == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        // update position
        var worldPos = guest.transform.position + offset;
        transform.position = Util.WorldToUISpace(CanvasController.I.MainCanvas, worldPos);
        
        // fade in/fade out
        var guestState = guest.CurrentState;
        if (guestState == Guest.GuestState.WaitingForService && !isShown)
        {
            wishBackground.color = Color.white;
            isShown = true;
            StartCoroutine(FadeTo(1f, fadeDuration));
        }
        else if (guestState != Guest.GuestState.WaitingForService && isShown)
        {
            isShown = false;
            StartCoroutine(FadeTo(0f, fadeDuration));
        }
        
        // update progress
        var currentWish = guest.CurrentWish;
        if (guestState == Guest.GuestState.WaitingForService && currentWish != null)
        {
            wishBackground.color = Color.Lerp(Color.white, Color.red, guest.CurrentWish.Progress);
            // todo: update wish icon
            //wishImage.sprite = "";
        }
    }

    #endregion

    #region Public

    public void SetGuest(Guest myGuest)
    {
        guest = myGuest;
        gameObject.SetActive(true);
    }

    #endregion

    #region Private

    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = canvasGroup.alpha;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            canvasGroup.alpha = Mathf.Lerp(alpha,aValue,t);
            yield return null;
        }
    }

    #endregion
}
