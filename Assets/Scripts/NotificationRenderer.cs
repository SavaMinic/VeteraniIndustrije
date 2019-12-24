using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationRenderer : MonoBehaviour
{
    #region Fields

    public Text notificationText;
    public RectTransform starsPanel;
    public Image[] starImages;
    public float initialDelay;
    public float fadeOutTime = 0.4f;
    public float moveTime = 0.8f;
    public float stayDuration = 4f;
    public float sizePerIndex = 200f;
    public Image iconImage;
    public Image guestIcon;
    public float sizeOffset = -100f;

    //public Sprite[] iconImageSprites;

    public AnimationCurve MoveAnimation;
    public AnimationCurve MoveOutAnimation;

    public CanvasGroup mainCanvasGroup;
    public RectTransform mainRectTransform;

    public Color goodColor;
    public Color badColor;

    private int myIndex;

    #endregion

    #region Mono

    private void Start()
    {
        mainCanvasGroup.alpha = 0f;
    }

    #endregion

    #region Public

    public void Show(int index, Guest guest, string text, int numberOfStars = -1, int maxStars = -1)
    {
        myIndex = index;
        starsPanel.gameObject.SetActive(numberOfStars >= 0);
        notificationText.text = text.ToUpper();

        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].color = i < numberOfStars ? goodColor : badColor;
            starImages[i].gameObject.SetActive(i < maxStars);
        }

        Debug.Assert(guest, "Guest is null");
        Debug.Assert(guest.spriteRenderer, "Guest renderer is null");
        guestIcon.sprite = guest.spriteRenderer.sprite;
        //iconImage.sprite = iconImageSprites[(int)guest.Type];

        SetVerticalPosition(-sizePerIndex);
        StartCoroutine(AnimateNotification(0, 1200f));
        StartCoroutine(AnimateNotification(sizePerIndex * index, 1200f));
    }

    #endregion

    #region Private

    private void SetVerticalPosition(float verticalPos)
    {
        var anchPos = mainRectTransform.anchoredPosition;
        anchPos.x = 0f;
        anchPos.y = verticalPos;
        mainRectTransform.anchoredPosition = anchPos;
    }

    IEnumerator FadeInAddMove(float stayPos, float aTime)
    {
        float alpha = mainCanvasGroup.alpha;
        float startPos = mainRectTransform.anchoredPosition.y;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            mainCanvasGroup.alpha = Mathf.Lerp(alpha, 1f, t);
            SetVerticalPosition(Mathf.LerpUnclamped(startPos, stayPos, MoveAnimation.Evaluate(t)));
            yield return null;
        }
    }

    IEnumerator FadeOutAndMove(float endPos, float aTime)
    {
        float alpha = mainCanvasGroup.alpha;
        float startPos = mainRectTransform.anchoredPosition.y;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            mainCanvasGroup.alpha = Mathf.Lerp(alpha, 0f, t);
            SetVerticalPosition(Mathf.LerpUnclamped(startPos, endPos, MoveOutAnimation.Evaluate(t)));
            yield return null;
        }
    }

    IEnumerator AnimateNotification(float stayPos, float endPos)
    {
        yield return new WaitForSecondsRealtime(initialDelay * Random.Range(0.8f, 1.2f));

        // fade and move
        yield return FadeInAddMove(stayPos, moveTime);

        // stay there
        yield return new WaitForSecondsRealtime(stayDuration);

        CanvasController.I.NotificationEnded(myIndex);

        // fade out
        yield return FadeOutAndMove(endPos, fadeOutTime);

        // bye
        Destroy(gameObject);
    }

    #endregion

}
