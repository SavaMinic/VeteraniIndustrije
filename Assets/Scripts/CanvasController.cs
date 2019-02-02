using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    #region Simple singleton

    private static CanvasController instance;

    public static CanvasController I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CanvasController>();
            }
            return instance;
        }
    }

    #endregion

    #region Properties/Fields

    public GameObject guestWishPanelPrefab;
    public Transform wishPanel;

    public RectTransform notificationsPanel;

    public Text starsCountText;
    public Text levelNameText;

    public GameObject notificationRendererPrefab;

    public Text promajaAlertLabel;
    public Text flekeAlertLabel;

    private List<GuestWishPanel> guestWishPanels = new List<GuestWishPanel>();

    public Canvas MainCanvas { get; private set; }

    private List<int> availableNotificationIndexes;

    private int starsCount;

    public CanvasGroup endGamePanel;
    public Text endGameScore;
    public Button closeButton;

    #endregion

    #region Mono

    private void Awake()
    {
        Application.targetFrameRate = 60;
        MainCanvas = GetComponent<Canvas>();
        availableNotificationIndexes = new List<int> { 0, 1, 2, 3 };
    }

    private void Start()
    {
        closeButton.onClick.AddListener(OnCloseClick);

        levelNameText.text = GameController.I.Level.Name ?? "NIVO TEST";
    }

    private void Update()
    {
        if (!Application.isPlaying)
            return;
        
        promajaAlertLabel.gameObject.SetActive(Promaja.IsActive);
        flekeAlertLabel.gameObject.SetActive(Fleka.Prljavo);
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(OnCloseClick);
    }

    #endregion

    #region Public

    public void ShowEndPanel()
    {
        endGameScore.text = "PROSLAVILI STE " + starsCount + " ŽELJA";
        StartCoroutine(FadeIn(0.4f));
    }

    public void AddStars(int stars)
    {
        starsCount += stars;
        starsCountText.text = "SLAVA: " + starsCount;
    }

    public void AddNewGuestWish(Guest guest)
    {
        var availableWishPanel = guestWishPanels.Find(p => !p.gameObject.activeSelf);
        if (availableWishPanel == null)
        {
            var wishPanelObject = Instantiate(guestWishPanelPrefab);
            wishPanelObject.transform.SetParent(wishPanel);
            wishPanelObject.transform.localScale = Vector3.one;
            availableWishPanel = wishPanelObject.GetComponent<GuestWishPanel>();
            guestWishPanels.Add(availableWishPanel);
        }
        availableWishPanel.SetGuest(guest);
    }

    public void ShowNotification(Guest guest, string text, int numberOfStars = -1, int maxStars = -1)
    {
        if (availableNotificationIndexes.Count <= 0) return;

        var notificationObject = Instantiate(notificationRendererPrefab);
        notificationObject.transform.SetParent(notificationsPanel.transform);
        notificationObject.transform.localScale = Vector3.one;

        var notification = notificationObject.GetComponent<NotificationRenderer>();

        var index = availableNotificationIndexes[0];
        availableNotificationIndexes.RemoveAt(0);
        notification.Show(index, guest, text, numberOfStars, maxStars);
    }

    public void NotificationEnded(int index)
    {
        availableNotificationIndexes.Add(index);
        availableNotificationIndexes.Sort();
    }

    #endregion
    
    #region Private
    
    private IEnumerator FadeIn(float time)
    {
        endGamePanel.interactable = endGamePanel.blocksRaycasts = true;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {
            endGamePanel.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
    }

    private void OnCloseClick()
    {
        SceneManager.LoadScene("MenuScene");
    }
    
    #endregion
}
