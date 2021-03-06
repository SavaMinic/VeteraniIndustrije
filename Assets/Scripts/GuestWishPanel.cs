﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuestWishPanel : MonoBehaviour
{
    #region Enums

    [System.Serializable]
    public class GuestWishSprite
    {
        public GuestWish.GuestWishType WishType;
        public Sprite WishSprite;
    }

    #endregion

    #region Properties/Fields

    [SerializeField] private Vector3 offset = default;
    [SerializeField] private List<GuestWishSprite> guestWishSprites = null;
    [SerializeField] private float fadeDuration = 0;
    [SerializeField] private Image wishImage = null;
    [SerializeField] private Image wishFillImage = null;
    [SerializeField] private Color successColor = default;
    [SerializeField] private Color failureColor = default;

    private Color timingColor;

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
        timingColor = wishFillImage.color;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void Update()
    {

        if (!Application.isPlaying || GameController.I.IsPaused || guest == null)
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
        if (guestState == Guest.GuestState.GoingOut)
        {
            guest = null;
            gameObject.SetActive(false);
            return;
        }

        if ((guestState == Guest.GuestState.WaitingForService || guestState == Guest.GuestState.WaitingForZito)
            && !isShown)
        {
            wishImage.color = Color.white;
            wishFillImage.color = timingColor;
            isShown = true;
            StartCoroutine(FadeTo(1f, fadeDuration));
        }
        else if ((guestState != Guest.GuestState.WaitingForService && guestState != Guest.GuestState.WaitingForZito)
            && isShown)
        {
            isShown = false;
            var lastFinishedWish = guest.LastFinishedWish;
            var delay = 0f;
            if (lastFinishedWish != null && lastFinishedWish.IsSuccess.HasValue)
            {
                wishFillImage.fillAmount = 1f;
                wishFillImage.color = lastFinishedWish.IsSuccess.Value ? successColor : failureColor;
                delay = 0.3f;
            }
            StartCoroutine(FadeTo(0f, fadeDuration, delay));
        }

        // update progress
        var currentWish = guest.CurrentWish;
        if ((guestState == Guest.GuestState.WaitingForService || guestState == Guest.GuestState.WaitingForZito)
            && currentWish != null)
        {
            var guestWishSprite = guestWishSprites.Find(w => w.WishType == currentWish.Type);
            if (guestWishSprite != null)
            {
                wishImage.sprite = guestWishSprite.WishSprite;
                wishFillImage.sprite = guestWishSprite.WishSprite;
            }
            wishFillImage.fillAmount = guest.CurrentWish.Progress;
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

    IEnumerator FadeTo(float aValue, float aTime, float delay = 0f)
    {
        if (delay > 0)
        {
            yield return new WaitForSecondsRealtime(delay);
        }
        float alpha = canvasGroup.alpha;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            canvasGroup.alpha = Mathf.Lerp(alpha, aValue, t);
            yield return null;
        }
        canvasGroup.alpha = aValue;
    }

    #endregion
}
